using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services;

/// <summary>
/// Database storage for stories using Entity Framework Core with SQLite.
/// </summary>
public class StoryStorageService
{
    private readonly AppDbContext _context;
    private readonly ILogger<StoryStorageService> _logger;

    public StoryStorageService(AppDbContext context, ILogger<StoryStorageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Story> SaveStoryAsync(Story story)
    {
        // Set StoryId for all pages
        foreach (var page in story.Pages)
        {
            page.StoryId = story.Id;
        }

        var existingStory = await _context.Stories
            .Include(s => s.Pages)
            .FirstOrDefaultAsync(s => s.Id == story.Id);

        if (existingStory == null)
        {
            _context.Stories.Add(story);
        }
        else
        {
            // Update existing story
            existingStory.Title = story.Title;
            existingStory.Description = story.Description;
            existingStory.ChildName = story.ChildName;
            existingStory.ChildAge = story.ChildAge;
            existingStory.Theme = story.Theme;
            existingStory.CoverImageUrl = story.CoverImageUrl;
            existingStory.IsFavorite = story.IsFavorite;
            existingStory.IsPublic = story.IsPublic;
            existingStory.ShareToken = story.ShareToken;
            existingStory.UpdatedAt = DateTime.UtcNow;

            // Update pages
            _context.StoryPages.RemoveRange(existingStory.Pages);
            existingStory.Pages = story.Pages;
            foreach (var page in existingStory.Pages)
            {
                page.StoryId = existingStory.Id;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Story saved: {story.Id} - {story.Title}");
        return story;
    }

    // Synchronous wrapper for backward compatibility
    public Story SaveStory(Story story)
    {
        return SaveStoryAsync(story).GetAwaiter().GetResult();
    }

    public async Task<Story?> GetStoryAsync(Guid id)
    {
        return await _context.Stories
            .Include(s => s.Pages.OrderBy(p => p.PageNumber))
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Story? GetStory(Guid id)
    {
        return GetStoryAsync(id).GetAwaiter().GetResult();
    }

    public async Task<List<Story>> GetAllStoriesAsync(Guid? userId = null)
    {
        var query = _context.Stories
            .Include(s => s.Pages.OrderBy(p => p.PageNumber))
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(s => s.UserId == userId);
        }

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public List<Story> GetAllStories()
    {
        return GetAllStoriesAsync().GetAwaiter().GetResult();
    }

    public async Task<bool> DeleteStoryAsync(Guid id)
    {
        var story = await _context.Stories.FindAsync(id);
        if (story == null)
        {
            return false;
        }

        _context.Stories.Remove(story);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Story deleted: {id}");
        return true;
    }

    public bool DeleteStory(Guid id)
    {
        return DeleteStoryAsync(id).GetAwaiter().GetResult();
    }

    public async Task<List<Story>> GetStoriesByChildNameAsync(string childName, Guid? userId = null)
    {
        var query = _context.Stories
            .Include(s => s.Pages.OrderBy(p => p.PageNumber))
            .Where(s => s.ChildName.ToLower() == childName.ToLower());

        if (userId.HasValue)
        {
            query = query.Where(s => s.UserId == userId);
        }

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public List<Story> GetStoriesByChildName(string childName)
    {
        return GetStoriesByChildNameAsync(childName).GetAwaiter().GetResult();
    }

    // New methods for enhanced functionality

    public async Task<Story?> GetStoryByShareTokenAsync(string shareToken)
    {
        return await _context.Stories
            .Include(s => s.Pages.OrderBy(p => p.PageNumber))
            .FirstOrDefaultAsync(s => s.ShareToken == shareToken && s.IsPublic);
    }

    public async Task<string> GenerateShareTokenAsync(Guid storyId)
    {
        var story = await _context.Stories.FindAsync(storyId);
        if (story == null)
        {
            throw new ArgumentException("Story not found");
        }

        story.ShareToken = Guid.NewGuid().ToString("N")[..12]; // 12 char token
        story.IsPublic = true;
        await _context.SaveChangesAsync();
        return story.ShareToken;
    }

    public async Task<bool> ToggleFavoriteAsync(Guid storyId)
    {
        var story = await _context.Stories.FindAsync(storyId);
        if (story == null)
        {
            return false;
        }

        story.IsFavorite = !story.IsFavorite;
        await _context.SaveChangesAsync();
        return story.IsFavorite;
    }

    public async Task<List<Story>> SearchStoriesAsync(
        string? searchTerm = null,
        string? theme = null,
        string? childName = null,
        bool? isFavorite = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? userId = null)
    {
        var query = _context.Stories
            .Include(s => s.Pages.OrderBy(p => p.PageNumber))
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(s => s.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(s => 
                s.Title.ToLower().Contains(term) || 
                s.Description.ToLower().Contains(term) ||
                s.ChildName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(theme))
        {
            query = query.Where(s => s.Theme.ToLower().Contains(theme.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(childName))
        {
            query = query.Where(s => s.ChildName.ToLower() == childName.ToLower());
        }

        if (isFavorite.HasValue)
        {
            query = query.Where(s => s.IsFavorite == isFavorite.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(s => s.CreatedAt <= toDate.Value);
        }

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<Story?> UpdateStoryPageAsync(Guid storyId, int pageNumber, string? content = null, string? imageUrl = null)
    {
        var story = await _context.Stories
            .Include(s => s.Pages)
            .FirstOrDefaultAsync(s => s.Id == storyId);

        if (story == null)
        {
            return null;
        }

        var page = story.Pages.FirstOrDefault(p => p.PageNumber == pageNumber);
        if (page == null)
        {
            return null;
        }

        if (content != null)
        {
            page.Content = content;
        }

        if (imageUrl != null)
        {
            page.ImageUrl = imageUrl;
        }

        story.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return story;
    }
}

