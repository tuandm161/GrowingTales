using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services;

public class SubscriptionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(AppDbContext context, ILogger<SubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserLimitsDto> GetUserLimitsAsync(Guid? userId)
    {
        // For guest users (not logged in)
        if (!userId.HasValue)
        {
            return new UserLimitsDto
            {
                Plan = "Guest",
                StoriesCreated = 0,
                StoriesRemaining = PlanLimits.Free.MaxStoriesTotal,
                MaxStoriesAllowed = PlanLimits.Free.MaxStoriesTotal,
                MaxPagesPerStory = PlanLimits.Free.MaxPagesPerStory,
                CanCreateStory = true,
                HasWatermark = true,
                UpgradeMessage = "Đăng ký để lưu trữ truyện và nhận thêm nhiều ưu đãi!"
            };
        }

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null)
        {
            return new UserLimitsDto
            {
                Plan = "Guest",
                CanCreateStory = true,
                MaxPagesPerStory = PlanLimits.Free.MaxPagesPerStory,
                HasWatermark = true
            };
        }

        // Reset monthly count if needed
        await ResetMonthlyCountIfNeeded(user);

        var isPremium = user.CurrentPlan == SubscriptionPlan.Premium && await HasActiveSubscription(user.Id);

        if (isPremium)
        {
            return new UserLimitsDto
            {
                Plan = "Premium",
                StoriesCreated = user.StoriesCreatedTotal,
                StoriesRemaining = int.MaxValue,
                MaxStoriesAllowed = int.MaxValue,
                MaxPagesPerStory = PlanLimits.Premium.MaxPagesPerStory,
                CanCreateStory = true,
                HasWatermark = false,
                UpgradeMessage = null
            };
        }

        // Free plan
        var remaining = PlanLimits.Free.MaxStoriesTotal - user.StoriesCreatedTotal;
        var canCreate = remaining > 0;

        return new UserLimitsDto
        {
            Plan = "Free",
            StoriesCreated = user.StoriesCreatedTotal,
            StoriesRemaining = Math.Max(0, remaining),
            MaxStoriesAllowed = PlanLimits.Free.MaxStoriesTotal,
            MaxPagesPerStory = PlanLimits.Free.MaxPagesPerStory,
            CanCreateStory = canCreate,
            HasWatermark = true,
            UpgradeMessage = canCreate 
                ? $"Còn {remaining} truyện miễn phí. Nâng cấp Premium để tạo không giới hạn!"
                : "Bạn đã hết lượt tạo truyện miễn phí. Nâng cấp Premium để tiếp tục!"
        };
    }

    public async Task<bool> CanUserCreateStory(Guid? userId)
    {
        var limits = await GetUserLimitsAsync(userId);
        return limits.CanCreateStory;
    }

    public async Task<int> GetMaxPagesForUser(Guid? userId)
    {
        var limits = await GetUserLimitsAsync(userId);
        return limits.MaxPagesPerStory;
    }

    public async Task IncrementStoryCount(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        await ResetMonthlyCountIfNeeded(user);

        user.StoriesCreatedTotal++;
        user.StoriesCreatedThisMonth++;
        user.LastStoryCreatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"User {userId} story count incremented to {user.StoriesCreatedTotal}");
    }

    public async Task<bool> HasActiveSubscription(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync();

        if (subscription == null) return false;

        // Check if subscription has expired
        if (subscription.EndDate.HasValue && subscription.EndDate.Value < DateTime.UtcNow)
        {
            subscription.Status = SubscriptionStatus.Expired;
            
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.CurrentPlan = SubscriptionPlan.Free;
            }
            
            await _context.SaveChangesAsync();
            return false;
        }

        return true;
    }

    public async Task<SubscriptionDto?> GetActiveSubscription(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync();

        if (subscription == null) return null;

        var daysRemaining = subscription.EndDate.HasValue
            ? (int)(subscription.EndDate.Value - DateTime.UtcNow).TotalDays
            : 0;

        return new SubscriptionDto
        {
            Id = subscription.Id,
            Plan = subscription.Plan.ToString(),
            Status = subscription.Status.ToString(),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            IsActive = subscription.Status == SubscriptionStatus.Active && daysRemaining >= 0,
            DaysRemaining = Math.Max(0, daysRemaining)
        };
    }

    public async Task<Subscription> CreateSubscription(Guid userId, string transactionId, decimal amount)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var subscription = new Subscription
        {
            UserId = userId,
            Plan = SubscriptionPlan.Premium,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            TransactionId = transactionId,
            AmountPaid = amount,
            PaymentMethod = "vnpay"
        };

        _context.Subscriptions.Add(subscription);

        // Update user plan
        user.CurrentPlan = SubscriptionPlan.Premium;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Subscription created for user {userId}, transaction: {transactionId}");

        return subscription;
    }

    public async Task<bool> ExtendSubscription(Guid userId, string transactionId, decimal amount)
    {
        var activeSubscription = await _context.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync();

        if (activeSubscription != null && activeSubscription.EndDate > DateTime.UtcNow)
        {
            // Extend existing subscription
            activeSubscription.EndDate = activeSubscription.EndDate.Value.AddMonths(1);
            activeSubscription.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Create new subscription
            await CreateSubscription(userId, transactionId, amount);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task ResetMonthlyCountIfNeeded(User user)
    {
        var currentMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        
        if (user.MonthResetDate < currentMonthStart)
        {
            user.StoriesCreatedThisMonth = 0;
            user.MonthResetDate = currentMonthStart;
            await _context.SaveChangesAsync();
        }
    }
}
