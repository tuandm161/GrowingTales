using System.Text.Json.Serialization;

namespace Server.Models;

public class Story
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<StoryPage> Pages { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public int ChildAge { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    
    // Trạng thái
    public StoryStatus Status { get; set; } = StoryStatus.Active;
    
    // Chia sẻ và yêu thích
    public bool IsFavorite { get; set; } = false;
    public string? ShareToken { get; set; }
    public bool IsPublic { get; set; } = false;
    
    // Thống kê
    public int ViewCount { get; set; } = 0;
    public int ShareCount { get; set; } = 0;
    public int DownloadCount { get; set; } = 0;
    
    // Ngôn ngữ
    public string Language { get; set; } = "vi";
    
    // User relationship (optional - for authenticated users)
    public Guid? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}

public class StoryPage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PageNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ImagePrompt { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    
    // Foreign key
    public Guid StoryId { get; set; }
    [JsonIgnore]
    public Story? Story { get; set; }
}

public enum StoryStatus
{
    Active = 0,
    Draft = 1,
    Archived = 2,
    Deleted = 3
}

// DTOs for statistics
public class StoryStatsDto
{
    public int TotalStories { get; set; }
    public int TotalViews { get; set; }
    public int TotalShares { get; set; }
    public int TotalDownloads { get; set; }
    public int StoriesThisMonth { get; set; }
    public int StoriesThisWeek { get; set; }
    public int StoriesYesterday { get; set; }
    public int StoresToday { get; set; }
    public List<ThemeStatsDto> ThemeStats { get; set; } = new();
}

public class ThemeStatsDto
{
    public string Theme { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}