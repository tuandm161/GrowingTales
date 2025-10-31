using Server.Models;

namespace Server.Services;

public class ImagePromptService
{
    public string BuildImagePrompt(Story story, StoryPage page)
    {
        // Build a distinct image prompt emphasizing cute, wholesome, vibrant cartoon style
        var themePart = string.IsNullOrWhiteSpace(story.Theme) ? string.Empty : $", theme: {story.Theme}";
        var childPart = string.IsNullOrWhiteSpace(story.ChildName) ? string.Empty : $", child name: {story.ChildName}, age: {story.ChildAge}";

        var sceneDescription = page.Content?.Trim() ?? string.Empty;

        var prompt =
            $"Vibrant cartoon illustration, cute and wholesome, children's picture book, soft lighting, bright colors, high detail, 2D animation style, Ghibli-inspired, no text, full scene{themePart}{childPart}. Scene: {sceneDescription}";

        return prompt;
    }
}


