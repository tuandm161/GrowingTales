namespace Server.Models;

public class StoryRequest
{
    public string InputText { get; set; } = string.Empty;
    public string ChildName { get; set; } = string.Empty;
    public int ChildAge { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string AdditionalContext { get; set; } = string.Empty;
    public int PageCount { get; set; } = 5;
    public string Language { get; set; } = "vi"; // Vietnamese by default
}

public class AudioStoryRequest
{
    public string AudioBase64 { get; set; } = string.Empty;
    public string ChildName { get; set; } = string.Empty;
    public int ChildAge { get; set; }
    public string Theme { get; set; } = string.Empty;
    public int PageCount { get; set; } = 5;
    public string Language { get; set; } = "vi";
}

public class StoryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Story? Story { get; set; }
}

