using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using Server.ViewModels;

namespace Server.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly FileUploadService _fileUploadService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AuthService authService, FileUploadService fileUploadService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, message, user) = await _authService.LoginAsync(model.Email, model.Password);

        if (!success || user == null)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        var principal = _authService.CreateClaimsPrincipal(user);
        
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

        _logger.LogInformation($"User {user.Email} logged in");

        TempData["SuccessMessage"] = "Đăng nhập thành công!";

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Trim email before validation
        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            model.Email = model.Email.Trim();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Registration failed validation. Errors: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(model);
        }

        var request = new RegisterRequest
        {
            Email = model.Email.Trim().ToLowerInvariant(),
            Password = model.Password,
            DisplayName = string.IsNullOrWhiteSpace(model.DisplayName) 
                ? model.Email.Split('@')[0] 
                : model.DisplayName.Trim()
        };

        var (success, message, user) = await _authService.RegisterAsync(request);

        if (!success || user == null)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        // Auto login after registration
        var principal = _authService.CreateClaimsPrincipal(user);
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        _logger.LogInformation($"User {user.Email} registered and logged in");

        TempData["SuccessMessage"] = "Đăng ký thành công! Chào mừng bạn đến với GrowingTales.";

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    // GET: /Account/Profile
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login");
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var viewModel = new EditProfileViewModel
        {
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            CurrentAvatarUrl = user.AvatarUrl
        };

        return View(viewModel);
    }

    // POST: /Account/Profile
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(EditProfileViewModel model)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login");
        }

        // Handle file upload
        string? avatarUrl = model.AvatarUrl;
        
        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            try
            {
                // Get current user to delete old avatar if exists
                var currentUser = await _authService.GetUserByIdAsync(userId);
                if (currentUser != null && !string.IsNullOrEmpty(currentUser.AvatarUrl))
                {
                    // Delete old uploaded file (not external URLs)
                    _fileUploadService.DeleteAvatar(currentUser.AvatarUrl);
                }

                // Upload new avatar
                var uploadedUrl = await _fileUploadService.UploadAvatarAsync(model.AvatarFile, userId);
                if (!string.IsNullOrEmpty(uploadedUrl))
                {
                    avatarUrl = uploadedUrl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                ModelState.AddModelError("AvatarFile", ex.Message);
                model.CurrentAvatarUrl = (await _authService.GetUserByIdAsync(userId))?.AvatarUrl;
                return View(model);
            }
        }

        if (!ModelState.IsValid)
        {
            model.CurrentAvatarUrl = (await _authService.GetUserByIdAsync(userId))?.AvatarUrl;
            return View(model);
        }

        var (success, message, user) = await _authService.UpdateProfileAsync(
            userId, 
            model.DisplayName, 
            model.PhoneNumber, 
            avatarUrl
        );

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        // Update claims if user updated
        if (user != null)
        {
            var principal = _authService.CreateClaimsPrincipal(user);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties 
                { 
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });
        }

        TempData["SuccessMessage"] = message;
        return RedirectToAction("Profile");
    }

    // GET: /Account/ChangePassword
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    // POST: /Account/ChangePassword
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return RedirectToAction("Login");
        }

        var (success, message) = await _authService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        TempData["SuccessMessage"] = message;
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/ForgotPassword
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        // Clear any unrelated TempData messages (like logout message)
        // Only keep messages that are specifically for ForgotPassword
        if (TempData["SuccessMessage"] != null && !TempData.ContainsKey("ForgotPasswordMessage"))
        {
            TempData.Remove("SuccessMessage");
        }
        if (TempData["ErrorMessage"] != null && !TempData.ContainsKey("ForgotPasswordError"))
        {
            TempData.Remove("ErrorMessage");
        }

        return View(new ForgotPasswordViewModel());
    }

    // POST: /Account/ForgotPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Trim email
        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            model.Email = model.Email.Trim();
        }

        var (success, message) = await _authService.GeneratePasswordResetTokenAsync(model.Email);

        if (success)
        {
            TempData["ForgotPasswordMessage"] = message;
            TempData["InfoMessage"] = "Vui lòng kiểm tra email của bạn để nhận link đặt lại mật khẩu. Nếu không thấy email, hãy kiểm tra thư mục spam.";
        }
        else
        {
            TempData["ForgotPasswordError"] = message;
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        return RedirectToAction("ForgotPassword");
    }

    // GET: /Account/ResetPassword
    [HttpGet]
    public IActionResult ResetPassword(string? token, string? email)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = "Link đặt lại mật khẩu không hợp lệ";
            return RedirectToAction("Login");
        }

        var viewModel = new ResetPasswordViewModel
        {
            Token = token,
            Email = email
        };

        return View(viewModel);
    }

    // POST: /Account/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, message) = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        TempData["SuccessMessage"] = message;
        return RedirectToAction("Login");
    }
}
