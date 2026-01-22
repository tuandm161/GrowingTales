using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly EmailService? _emailService;

    public AuthService(AppDbContext context, ILogger<AuthService> logger, EmailService? emailService = null)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Trim and normalize email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return (false, "Email không được để trống", null);
            }

            var email = request.Email.Trim().ToLowerInvariant();

            // Validate email format
            if (!IsValidEmail(email))
            {
                _logger.LogWarning("Invalid email format: {Email}", email);
                return (false, "Email không hợp lệ. Vui lòng nhập đúng định dạng email (ví dụ: name@example.com)", null);
            }

            // Validate email length (RFC 5321)
            if (email.Length > 254)
            {
                return (false, "Email không được quá 254 ký tự", null);
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email))
            {
                _logger.LogWarning("Email already exists: {Email}", email);
                return (false, "Email đã được sử dụng. Vui lòng sử dụng email khác hoặc đăng nhập", null);
            }

            // Validate password
            if (request.Password.Length < 6)
            {
                return (false, "Mật khẩu phải có ít nhất 6 ký tự", null);
            }

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) 
                    ? email.Split('@')[0] 
                    : request.DisplayName.Trim()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User registered: {user.Email}");

            return (true, "Đăng ký thành công!", user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return (false, "Lỗi khi đăng ký: " + ex.Message, null);
        }
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return (false, "Email hoặc mật khẩu không đúng", null);
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, "Email hoặc mật khẩu không đúng", null);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User logged in: {user.Email}");

            return (true, "Đăng nhập thành công!", user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return (false, "Lỗi khi đăng nhập: " + ex.Message, null);
        }
    }

    public ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName),
            new("Plan", user.CurrentPlan.ToString())
        };

        // Add avatar URL to claims if available
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            claims.Add(new Claim("AvatarUrl", user.AvatarUrl));
        }

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        return new ClaimsPrincipal(identity);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal claims)
    {
        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return await _context.Users.FindAsync(userId);
    }

    public Guid? GetCurrentUserId(ClaimsPrincipal claims)
    {
        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Người dùng không tồn tại");
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return (false, "Mật khẩu hiện tại không đúng");
            }

            // Validate new password
            if (newPassword.Length < 6)
            {
                return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {user.Email} changed password");
            return (true, "Đổi mật khẩu thành công!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return (false, "Lỗi khi đổi mật khẩu: " + ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> GeneratePasswordResetTokenAsync(string email)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());

            if (user == null)
            {
                // Don't reveal if email exists or not for security
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                return (true, "Nếu email tồn tại, bạn sẽ nhận được link đặt lại mật khẩu");
            }

            // Generate reset token
            var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            user.PasswordResetToken = token;
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password reset token generated for user: {user.Email}");

            // Send email if email service is configured
            if (_emailService != null && _emailService.IsConfigured())
            {
                var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email, token, user.Email);
                if (emailSent)
                {
                    return (true, "Chúng tôi đã gửi link đặt lại mật khẩu đến email của bạn. Vui lòng kiểm tra hộp thư (cả thư mục spam).");
                }
                else
                {
                    _logger.LogWarning("Failed to send password reset email to {Email}", user.Email);
                    // Fallback: return token in message if email fails (for development)
                    var resetUrl = $"/Account/ResetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
                    return (true, $"Không thể gửi email. Link reset: {resetUrl}");
                }
            }
            else
            {
                // Email service not configured - return token for development
                _logger.LogWarning("Email service not configured. Returning reset link in response.");
                var resetUrl = $"/Account/ResetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
                return (true, $"Email service chưa được cấu hình. Link reset: {resetUrl}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating password reset token");
            return (false, "Lỗi khi tạo token đặt lại mật khẩu: " + ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());

            if (user == null)
            {
                return (false, "Email không tồn tại");
            }

            // Validate token
            if (string.IsNullOrEmpty(user.PasswordResetToken) || user.PasswordResetToken != token)
            {
                return (false, "Token đặt lại mật khẩu không hợp lệ");
            }

            // Check if token expired
            if (user.PasswordResetExpiry == null || user.PasswordResetExpiry < DateTime.UtcNow)
            {
                return (false, "Token đặt lại mật khẩu đã hết hạn. Vui lòng yêu cầu lại");
            }

            // Validate new password
            if (newPassword.Length < 6)
            {
                return (false, "Mật khẩu mới phải có ít nhất 6 ký tự");
            }

            // Update password and clear reset token
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {user.Email} reset password");
            return (true, "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập với mật khẩu mới.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return (false, "Lỗi khi đặt lại mật khẩu: " + ex.Message);
        }
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // Use MailAddress for basic validation
            var addr = new System.Net.Mail.MailAddress(email);
            
            // Ensure the address matches exactly (no display name)
            if (addr.Address != email)
            {
                return false;
            }

            // Additional checks
            var parts = email.Split('@');
            if (parts.Length != 2)
            {
                return false;
            }

            var localPart = parts[0];
            var domainPart = parts[1];

            // Local part validation
            if (string.IsNullOrWhiteSpace(localPart) || localPart.Length > 64)
            {
                return false;
            }

            // Domain part validation
            if (string.IsNullOrWhiteSpace(domainPart) || domainPart.Length > 253)
            {
                return false;
            }

            // Check for valid domain format (at least one dot)
            if (!domainPart.Contains('.'))
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string Message, User? User)> UpdateProfileAsync(Guid userId, string displayName, string? phoneNumber, string? avatarUrl)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Người dùng không tồn tại", null);
            }

            // Validate display name
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return (false, "Tên hiển thị không được để trống", null);
            }

            if (displayName.Length > 200)
            {
                return (false, "Tên hiển thị không được quá 200 ký tự", null);
            }

            // Update profile
            user.DisplayName = displayName.Trim();
            user.PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
            user.AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();
            user.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {user.Email} updated profile");
            return (true, "Cập nhật thông tin thành công!", user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return (false, "Lỗi khi cập nhật thông tin: " + ex.Message, null);
        }
    }
}
