using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return (false, "Email đã được sử dụng", null);
            }

            // Validate email format
            if (!IsValidEmail(request.Email))
            {
                return (false, "Email không hợp lệ", null);
            }

            // Validate password
            if (request.Password.Length < 6)
            {
                return (false, "Mật khẩu phải có ít nhất 6 ký tự", null);
            }

            var user = new User
            {
                Email = request.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) 
                    ? request.Email.Split('@')[0] 
                    : request.DisplayName
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

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
