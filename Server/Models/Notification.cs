using System.Text.Json.Serialization;

namespace Server.Models;

/// <summary>
/// Bảng thông báo cho người dùng
/// </summary>
public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Người nhận
    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
    
    // Nội dung
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    
    // Liên kết
    public string? Link { get; set; } // URL để điều hướng khi click
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    
    // Trạng thái
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    
    // Icon và hình ảnh
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    
    // Thời gian
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}

public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    
    // Specific types
    StoryCreated = 10,
    StoryShared = 11,
    
    SubscriptionExpiring = 20,
    SubscriptionExpired = 21,
    SubscriptionRenewed = 22,
    
    PaymentSuccess = 30,
    PaymentFailed = 31,
    
    WelcomeMessage = 40,
    PromoOffer = 41,
    
    SystemUpdate = 50,
    Maintenance = 51
}

// DTOs
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Icon { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationCountDto
{
    public int Total { get; set; }
    public int Unread { get; set; }
}
