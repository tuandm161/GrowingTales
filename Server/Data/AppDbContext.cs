using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Core entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Story> Stories { get; set; } = null!;
    public DbSet<StoryPage> StoryPages { get; set; } = null!;
    
    // Subscription & Payment
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    
    // System
    public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<SystemSetting> SystemSettings { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== USER ====================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.LastLoginIp).HasMaxLength(50);
            entity.Property(e => e.EmailConfirmToken).HasMaxLength(200);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(200);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // ==================== STORY ====================
        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ChildName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Theme).HasMaxLength(200);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(5000000);
            entity.Property(e => e.ShareToken).HasMaxLength(100);
            entity.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("vi");
            
            entity.HasMany(e => e.Pages)
                .WithOne(p => p.Story)
                .HasForeignKey(p => p.StoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Stories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ShareToken).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ChildName);
            entity.HasIndex(e => e.Theme);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ViewCount);
        });

        // ==================== STORY PAGE ====================
        modelBuilder.Entity<StoryPage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.ImagePrompt).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(5000000);

            entity.HasIndex(e => new { e.StoryId, e.PageNumber });
        });

        // ==================== SUBSCRIPTION ====================
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionId).HasMaxLength(200);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.AmountPaid).HasPrecision(18, 2);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransactionId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
        });

        // ==================== PAYMENT ====================
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(200);
            entity.Property(e => e.OrderId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.FinalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("VND");
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.BankCode).HasMaxLength(50);
            entity.Property(e => e.CardType).HasMaxLength(50);
            entity.Property(e => e.ResponseCode).HasMaxLength(50);
            entity.Property(e => e.ResponseMessage).HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Subscription)
                .WithMany()
                .HasForeignKey(e => e.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransactionId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Method);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.PaidAt);
        });

        // ==================== ACTIVITY LOG ====================
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.EntityName).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.Referer).HasMaxLength(500);
            entity.Property(e => e.RequestPath).HasMaxLength(500);
            entity.Property(e => e.RequestMethod).HasMaxLength(10);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);

            entity.HasOne(e => e.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsSuccess);
        });

        // ==================== NOTIFICATION ====================
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedAt);
        });

        // ==================== SYSTEM SETTING ====================
        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Group).HasMaxLength(50).HasDefaultValue("General");
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);

            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.Group);
        });

        // ==================== CONTACT ====================
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.AdminNote).HasMaxLength(2000);
            entity.Property(e => e.Response).HasMaxLength(5000);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // ==================== SEED DATA ====================
        SeedDefaultSettings(modelBuilder);
    }

    private void SeedDefaultSettings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemSetting>().HasData(
            // General Settings
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000001"),
                Key = "site_name",
                Value = "GrowingTales",
                Description = "Tên website",
                Type = SettingType.String,
                Group = "General",
                IsPublic = true
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000002"),
                Key = "site_description",
                Value = "Tạo truyện cho bé yêu với AI",
                Description = "Mô tả website",
                Type = SettingType.String,
                Group = "General",
                IsPublic = true
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000003"),
                Key = "contact_email",
                Value = "support@growingtales.vn",
                Description = "Email liên hệ",
                Type = SettingType.String,
                Group = "Contact",
                IsPublic = true
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000004"),
                Key = "contact_phone",
                Value = "0123 456 789",
                Description = "Số điện thoại liên hệ",
                Type = SettingType.String,
                Group = "Contact",
                IsPublic = true
            },
            
            // Pricing Settings
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000010"),
                Key = "premium_price",
                Value = "150000",
                Description = "Giá gói Premium (VND)",
                Type = SettingType.Number,
                Group = "Pricing"
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000011"),
                Key = "free_stories_limit",
                Value = "3",
                Description = "Số truyện miễn phí",
                Type = SettingType.Number,
                Group = "Pricing"
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000012"),
                Key = "free_pages_per_story",
                Value = "5",
                Description = "Số trang tối đa cho gói Free",
                Type = SettingType.Number,
                Group = "Pricing"
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000013"),
                Key = "premium_pages_per_story",
                Value = "15",
                Description = "Số trang tối đa cho gói Premium",
                Type = SettingType.Number,
                Group = "Pricing"
            },
            
            // Feature Flags
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000020"),
                Key = "enable_registration",
                Value = "true",
                Description = "Cho phép đăng ký tài khoản mới",
                Type = SettingType.Boolean,
                Group = "Features"
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000021"),
                Key = "enable_guest_stories",
                Value = "true",
                Description = "Cho phép khách tạo truyện không cần đăng ký",
                Type = SettingType.Boolean,
                Group = "Features"
            },
            new SystemSetting
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000022"),
                Key = "maintenance_mode",
                Value = "false",
                Description = "Chế độ bảo trì",
                Type = SettingType.Boolean,
                Group = "System"
            }
        );
    }
}
