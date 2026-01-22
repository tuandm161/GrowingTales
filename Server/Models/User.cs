using System.Text.Json.Serialization;

namespace Server.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Thông tin cơ bản
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    
    // Vai trò và trạng thái
    public UserRole Role { get; set; } = UserRole.User;
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    // Thời gian
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    
    // Subscription & Usage
    public SubscriptionPlan CurrentPlan { get; set; } = SubscriptionPlan.Free;
    public int StoriesCreatedTotal { get; set; } = 0;
    public int StoriesCreatedThisMonth { get; set; } = 0;
    public DateTime? LastStoryCreatedAt { get; set; }
    public DateTime MonthResetDate { get; set; } = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    
    // Xác thực
    public bool EmailConfirmed { get; set; } = false;
    public string? EmailConfirmToken { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public List<Story> Stories { get; set; } = new();
    [JsonIgnore]
    public List<Subscription> Subscriptions { get; set; } = new();
    [JsonIgnore]
    public List<Payment> Payments { get; set; } = new();
    [JsonIgnore]
    public List<ActivityLog> ActivityLogs { get; set; } = new();
    [JsonIgnore]
    public List<Notification> Notifications { get; set; } = new();
}

public enum UserRole
{
    User = 0,
    Admin = 1,
    SuperAdmin = 2
}

public enum UserStatus
{
    Active = 0,
    Inactive = 1,
    Banned = 2,
    Deleted = 3
}

// DTOs for authentication
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public UserDto? User { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; }
    public string Plan { get; set; } = "Free";
    public int StoriesRemaining { get; set; }
    public int TotalStories { get; set; }
}
