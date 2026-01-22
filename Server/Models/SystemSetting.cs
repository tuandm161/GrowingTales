namespace Server.Models;

/// <summary>
/// Bảng cấu hình hệ thống - lưu các settings có thể thay đổi từ admin
/// </summary>
public class SystemSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Key { get; set; } = string.Empty; // Unique key
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SettingType Type { get; set; } = SettingType.String;
    public string Group { get; set; } = "General"; // Nhóm setting
    
    public bool IsPublic { get; set; } = false; // Có hiển thị cho frontend không
    public bool IsEditable { get; set; } = true; // Admin có thể sửa không
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public enum SettingType
{
    String = 0,
    Number = 1,
    Boolean = 2,
    Json = 3,
    Html = 4
}

/// <summary>
/// Bảng liên hệ / phản hồi từ người dùng
/// </summary>
public class Contact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Thông tin người gửi
    public Guid? UserId { get; set; } // Null nếu khách
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    
    // Nội dung
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ContactType Type { get; set; } = ContactType.General;
    
    // Trạng thái xử lý
    public ContactStatus Status { get; set; } = ContactStatus.New;
    public string? AdminNote { get; set; }
    public string? Response { get; set; }
    public Guid? AssignedTo { get; set; } // Admin được giao xử lý
    
    // Thời gian
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    // Thông tin bổ sung
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public enum ContactType
{
    General = 0,      // Liên hệ chung
    Support = 1,      // Hỗ trợ kỹ thuật
    Feedback = 2,     // Góp ý
    Bug = 3,          // Báo lỗi
    Feature = 4,      // Yêu cầu tính năng
    Payment = 5,      // Vấn đề thanh toán
    Partnership = 6,  // Hợp tác
    Other = 99
}

public enum ContactStatus
{
    New = 0,          // Mới
    InProgress = 1,   // Đang xử lý
    Responded = 2,    // Đã phản hồi
    Closed = 3,       // Đã đóng
    Spam = 4          // Spam
}

// DTOs
public class SystemSettingDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "String";
    public string Group { get; set; } = "General";
}

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ContactFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "General";
}
