using System.Collections.Concurrent;
using Server.Models;

namespace Server.Services;

/// <summary>
/// In-memory storage for stories. In production, this should be replaced with a database.
/// </summary>
public class StoryStorageService
{
    private readonly ConcurrentDictionary<Guid, Story> _stories = new();

    public Story SaveStory(Story story)
    {
        _stories[story.Id] = story;
        return story;
    }

    public Story? GetStory(Guid id)
    {
        _stories.TryGetValue(id, out var story);
        return story;
    }

    public List<Story> GetAllStories()
    {
        return _stories.Values.OrderByDescending(s => s.CreatedAt).ToList();
    }

    public bool DeleteStory(Guid id)
    {
        return _stories.TryRemove(id, out _);
    }

    public List<Story> GetStoriesByChildName(string childName)
    {
        return _stories.Values
            .Where(s => s.ChildName.Equals(childName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }
}

