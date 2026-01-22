namespace Server.Models;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    
    // Payment info
    public string? TransactionId { get; set; }
    public decimal AmountPaid { get; set; }
    public string? PaymentMethod { get; set; } // "vnpay", "momo", etc.
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum SubscriptionPlan
{
    Free = 0,
    Premium = 1
}

public enum SubscriptionStatus
{
    Active = 0,
    Expired = 1,
    Cancelled = 2,
    Pending = 3
}

// User usage tracking
public class UserUsage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public int StoriesCreatedThisMonth { get; set; } = 0;
    public int TotalStoriesCreated { get; set; } = 0;
    public DateTime LastStoryCreatedAt { get; set; }
    public DateTime MonthStartDate { get; set; } = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
}

// Plan limits configuration
public static class PlanLimits
{
    public static class Free
    {
        public const int MaxStoriesTotal = 3;
        public const int MaxPagesPerStory = 5;
        public const bool HasWatermark = true;
        public const bool CanExportPdf = true;
        public const bool CanShare = true;
    }
    
    public static class Premium
    {
        public const int MaxStoriesPerMonth = int.MaxValue; // Unlimited
        public const int MaxPagesPerStory = 15;
        public const bool HasWatermark = false;
        public const bool CanExportPdf = true;
        public const bool CanShare = true;
        public const decimal MonthlyPrice = 150000; // 150,000 VND
    }
}

// DTOs
public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string Plan { get; set; } = "Free";
    public string Status { get; set; } = "Active";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int DaysRemaining { get; set; }
}

public class UserLimitsDto
{
    public string Plan { get; set; } = "Free";
    public int StoriesCreated { get; set; }
    public int StoriesRemaining { get; set; }
    public int MaxStoriesAllowed { get; set; }
    public int MaxPagesPerStory { get; set; }
    public bool CanCreateStory { get; set; }
    public bool HasWatermark { get; set; }
    public string? UpgradeMessage { get; set; }
}

public class CreatePaymentRequest
{
    public string Plan { get; set; } = "Premium";
    public string ReturnUrl { get; set; } = string.Empty;
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? PaymentUrl { get; set; }
    public string? TransactionId { get; set; }
}

public class VnPayCallbackRequest
{
    public string vnp_TxnRef { get; set; } = string.Empty;
    public string vnp_ResponseCode { get; set; } = string.Empty;
    public string vnp_TransactionNo { get; set; } = string.Empty;
    public string vnp_Amount { get; set; } = string.Empty;
    public string vnp_SecureHash { get; set; } = string.Empty;
}
