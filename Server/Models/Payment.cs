using System.Text.Json.Serialization;

namespace Server.Models;

/// <summary>
/// Bảng lưu trữ tất cả giao dịch thanh toán - dùng để theo dõi doanh thu
/// </summary>
public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Liên kết người dùng
    public Guid UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
    
    // Liên kết subscription (nếu có)
    public Guid? SubscriptionId { get; set; }
    [JsonIgnore]
    public Subscription? Subscription { get; set; }
    
    // Thông tin giao dịch
    public string TransactionId { get; set; } = string.Empty; // Mã giao dịch từ cổng thanh toán
    public string OrderId { get; set; } = string.Empty; // Mã đơn hàng nội bộ
    public PaymentMethod Method { get; set; } = PaymentMethod.VnPay;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentType Type { get; set; } = PaymentType.Subscription;
    
    // Số tiền
    public decimal Amount { get; set; } // Số tiền gốc
    public decimal Discount { get; set; } = 0; // Giảm giá
    public decimal FinalAmount { get; set; } // Số tiền thực trả
    public string Currency { get; set; } = "VND";
    
    // Thông tin gói
    public string? PlanName { get; set; } // "Premium", "Basic", etc.
    public int? DurationMonths { get; set; } // Số tháng mua
    
    // Mô tả
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    
    // Thông tin thanh toán chi tiết
    public string? BankCode { get; set; }
    public string? CardType { get; set; }
    public string? PayerInfo { get; set; } // JSON chứa thông tin người thanh toán
    
    // Response từ cổng thanh toán
    public string? GatewayResponse { get; set; } // JSON response
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }
    
    // Thời gian
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    
    // IP và thiết bị
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public enum PaymentMethod
{
    VnPay = 0,
    Momo = 1,
    ZaloPay = 2,
    BankTransfer = 3,
    Card = 4,
    Cash = 5,
    Other = 99
}

public enum PaymentStatus
{
    Pending = 0,      // Đang chờ thanh toán
    Processing = 1,   // Đang xử lý
    Completed = 2,    // Thành công
    Failed = 3,       // Thất bại
    Cancelled = 4,    // Đã hủy
    Refunded = 5,     // Đã hoàn tiền
    PartialRefund = 6 // Hoàn tiền một phần
}

public enum PaymentType
{
    Subscription = 0, // Mua gói
    Renewal = 1,      // Gia hạn
    Upgrade = 2,      // Nâng cấp
    Addon = 3,        // Mua thêm
    Refund = 4        // Hoàn tiền
}

// DTOs
public class PaymentDto
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? PlanName { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public UserDto? User { get; set; }
}

public class RevenueReportDto
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTransactions { get; set; }
    public int SuccessfulTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public decimal AvgTransactionValue { get; set; }
}

public class RevenueSummaryDto
{
    public decimal TodayRevenue { get; set; }
    public decimal ThisWeekRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public decimal ThisYearRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
    
    public int TodayTransactions { get; set; }
    public int ThisMonthTransactions { get; set; }
    public int TotalTransactions { get; set; }
    
    public int NewSubscriptionsToday { get; set; }
    public int NewSubscriptionsThisMonth { get; set; }
    public int TotalActiveSubscriptions { get; set; }
    
    public decimal GrowthRate { get; set; } // So với tháng trước (%)
}
