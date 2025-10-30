using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoryController : ControllerBase
{
    private readonly GeminiService _geminiService;
    private readonly StoryStorageService _storageService;
    private readonly ILogger<StoryController> _logger;

    public StoryController(
        GeminiService geminiService,
        StoryStorageService storageService,
        ILogger<StoryController> logger)
    {
        _geminiService = geminiService;
        _storageService = storageService;
        _logger = logger;
    }

    [HttpPost("generate-from-text")]
    public async Task<ActionResult<StoryResponse>> GenerateFromText([FromBody] StoryRequest request)
    {
        try
        {
            _logger.LogInformation($"Generating story for {request.ChildName}");

            var story = await _geminiService.GenerateStoryFromText(request);
            _storageService.SaveStory(story);

            return Ok(new StoryResponse
            {
                Success = true,
                Message = "Câu chuyện đã được tạo thành công!",
                Story = story
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating story from text");
            return StatusCode(500, new StoryResponse
            {
                Success = false,
                Message = $"Lỗi khi tạo câu chuyện: {ex.Message}"
            });
        }
    }

    [HttpPost("generate-from-audio")]
    public async Task<ActionResult<StoryResponse>> GenerateFromAudio([FromBody] AudioStoryRequest request)
    {
        try
        {
            _logger.LogInformation($"Generating story from audio for {request.ChildName}");

            var story = await _geminiService.GenerateStoryFromAudio(request);
            _storageService.SaveStory(story);

            return Ok(new StoryResponse
            {
                Success = true,
                Message = "Câu chuyện đã được tạo thành công từ giọng nói!",
                Story = story
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating story from audio");
            return StatusCode(500, new StoryResponse
            {
                Success = false,
                Message = $"Lỗi khi tạo câu chuyện từ audio: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public ActionResult<List<Story>> GetAllStories()
    {
        var stories = _storageService.GetAllStories();
        return Ok(stories);
    }

    [HttpGet("{id}")]
    public ActionResult<Story> GetStory(Guid id)
    {
        var story = _storageService.GetStory(id);
        if (story == null)
        {
            return NotFound(new { message = "Không tìm thấy câu chuyện" });
        }
        return Ok(story);
    }

    [HttpGet("by-child/{childName}")]
    public ActionResult<List<Story>> GetStoriesByChild(string childName)
    {
        var stories = _storageService.GetStoriesByChildName(childName);
        return Ok(stories);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteStory(Guid id)
    {
        var success = _storageService.DeleteStory(id);
        if (!success)
        {
            return NotFound(new { message = "Không tìm thấy câu chuyện" });
        }
        return Ok(new { message = "Đã xóa câu chuyện thành công" });
    }
}

