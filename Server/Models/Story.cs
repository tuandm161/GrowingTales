namespace Server.Models;

public class Story
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<StoryPage> Pages { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ChildName { get; set; } = string.Empty;
    public int ChildAge { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
}

public class StoryPage
{
    public int PageNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ImagePrompt { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

