using System.Text.Json.Serialization;

namespace Server.Models;

/// <summary>
/// Bảng ghi lại hoạt động của người dùng - dùng để theo dõi và phân tích
/// </summary>
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Liên kết người dùng (có thể null nếu là khách)
    public Guid? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
    
    // Thông tin hoạt động
    public ActivityType Type { get; set; }
    public string Action { get; set; } = string.Empty; // VD: "Login", "CreateStory", "ViewStory"
    public string Description { get; set; } = string.Empty;
    
    // Đối tượng liên quan
    public string? EntityType { get; set; } // "Story", "User", "Subscription"
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }
    
    // Dữ liệu bổ sung (JSON)
    public string? OldValues { get; set; } // Giá trị cũ (cho update/delete)
    public string? NewValues { get; set; } // Giá trị mới
    public string? AdditionalData { get; set; } // Dữ liệu khác
    
    // Thông tin request
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    
    // Kết quả
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
    
    // Thời gian
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ActivityType
{
    // Authentication
    Login = 0,
    Logout = 1,
    Register = 2,
    PasswordChange = 3,
    PasswordReset = 4,
    
    // Story
    StoryCreate = 10,
    StoryView = 11,
    StoryEdit = 12,
    StoryDelete = 13,
    StoryShare = 14,
    StoryFavorite = 15,
    StoryExportPdf = 16,
    
    // Subscription & Payment
    SubscriptionPurchase = 20,
    SubscriptionRenew = 21,
    SubscriptionCancel = 22,
    PaymentSuccess = 23,
    PaymentFailed = 24,
    
    // User Profile
    ProfileUpdate = 30,
    AvatarUpdate = 31,
    EmailChange = 32,
    
    // Admin
    AdminUserUpdate = 40,
    AdminUserBan = 41,
    AdminUserDelete = 42,
    AdminSettingChange = 43,
    
    // System
    SystemError = 90,
    ApiCall = 91,
    Other = 99
}

// DTOs
public class ActivityLogDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserName { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? IpAddress { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ActivitySummaryDto
{
    public int TotalLogins { get; set; }
    public int TotalRegistrations { get; set; }
    public int TotalStoriesCreated { get; set; }
    public int TotalStoriesViewed { get; set; }
    public int TotalPayments { get; set; }
    public DateTime? LastActivity { get; set; }
}
