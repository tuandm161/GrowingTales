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
    private readonly ILogger<AccountController> _logger;

    public AccountController(AuthService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password,
            DisplayName = model.DisplayName ?? model.Email.Split('@')[0]
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
}
