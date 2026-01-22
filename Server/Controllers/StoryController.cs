using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using Server.ViewModels;

namespace Server.Controllers;

public class StoryController : Controller
{
    private readonly GeminiService _geminiService;
    private readonly StoryStorageService _storageService;
    private readonly ImageGenerationService _imageService;
    private readonly ImagePromptService _imagePromptService;
    private readonly SubscriptionService _subscriptionService;
    private readonly ILogger<StoryController> _logger;

    public StoryController(
        GeminiService geminiService,
        StoryStorageService storageService,
        ImageGenerationService imageService,
        ImagePromptService imagePromptService,
        SubscriptionService subscriptionService,
        ILogger<StoryController> logger)
    {
        _geminiService = geminiService;
        _storageService = storageService;
        _imageService = imageService;
        _imagePromptService = imagePromptService;
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    // GET: /Story
    public async Task<IActionResult> Index(string? search, string? theme, string? childName, bool? favorite)
    {
        var userId = GetCurrentUserId();
        var stories = await _storageService.SearchStoriesAsync(
            searchTerm: search,
            theme: theme,
            childName: childName,
            isFavorite: favorite,
            userId: userId
        );

        var allStories = await _storageService.GetAllStoriesAsync(userId);
        
        var viewModel = new StoryListViewModel
        {
            Stories = stories,
            SearchTerm = search,
            FilterTheme = theme,
            FilterChildName = childName,
            FilterFavorite = favorite,
            AvailableThemes = allStories.Select(s => s.Theme).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList(),
            AvailableChildNames = allStories.Select(s => s.ChildName).Distinct().ToList()
        };

        return View(viewModel);
    }

    // GET: /Story/Create
    public async Task<IActionResult> Create()
    {
        var userId = GetCurrentUserId();
        var limits = await _subscriptionService.GetUserLimitsAsync(userId);
        
        if (!limits.CanCreateStory)
        {
            TempData["ErrorMessage"] = limits.UpgradeMessage;
            return RedirectToAction("Pricing", "Subscription");
        }

        ViewBag.Limits = limits;
        return View(new CreateStoryViewModel());
    }

    // POST: /Story/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStoryViewModel model)
    {
        _logger.LogInformation("Create story POST called. Model: ChildName={ChildName}, ChildAge={ChildAge}, UseAudio={UseAudio}", 
            model?.ChildName, model?.ChildAge, model?.UseAudio);

        var userId = GetCurrentUserId();
        var limits = await _subscriptionService.GetUserLimitsAsync(userId);
        
        if (!limits.CanCreateStory)
        {
            TempData["ErrorMessage"] = limits.UpgradeMessage;
            return RedirectToAction("Pricing", "Subscription");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid. Errors: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            ViewBag.Limits = limits;
            return View(model);
        }

        if (model == null)
        {
            _logger.LogWarning("Model is null in Create action");
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
            return RedirectToAction("Create");
        }

        try
        {
            Story story;

            // Validate: if UseAudio is true, AudioBase64 must be provided
            if (model.UseAudio && string.IsNullOrWhiteSpace(model.AudioBase64))
            {
                _logger.LogWarning("UseAudio is true but AudioBase64 is empty");
                ModelState.AddModelError("", "Vui lòng ghi âm hoặc nhập nội dung truyện");
                ViewBag.Limits = limits;
                return View(model);
            }

            // Validate: if not using audio, Content should be provided
            if (!model.UseAudio && string.IsNullOrWhiteSpace(model.Content))
            {
                _logger.LogWarning("Not using audio but Content is empty");
                ModelState.AddModelError("Content", "Vui lòng nhập nội dung hoặc ý tưởng cho truyện");
                ViewBag.Limits = limits;
                return View(model);
            }

            if (model.UseAudio && !string.IsNullOrEmpty(model.AudioBase64))
            {
                _logger.LogInformation("Generating story from audio");
                var audioRequest = new AudioStoryRequest
                {
                    ChildName = model.ChildName,
                    ChildAge = model.ChildAge,
                    Theme = model.Theme ?? "",
                    AudioBase64 = model.AudioBase64
                };
                story = await _geminiService.GenerateStoryFromAudio(audioRequest);
            }
            else
            {
                _logger.LogInformation("Generating story from text");
                var textRequest = new StoryRequest
                {
                    ChildName = model.ChildName,
                    ChildAge = model.ChildAge,
                    Theme = model.Theme ?? "",
                    InputText = model.Content ?? ""
                };
                story = await _geminiService.GenerateStoryFromText(textRequest);
            }

            story.UserId = userId;
            await _storageService.SaveStoryAsync(story);

            // Increment story count for user
            if (userId.HasValue)
            {
                await _subscriptionService.IncrementStoryCount(userId.Value);
            }

            TempData["SuccessMessage"] = "Truyện đã được tạo thành công!";
            return RedirectToAction("Details", new { id = story.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating story");
            ModelState.AddModelError("", "Lỗi khi tạo truyện: " + ex.Message);
            ViewBag.Limits = limits;
            return View(model);
        }
    }

    // GET: /Story/Details/{id}
    public async Task<IActionResult> Details(Guid id, int page = 1)
    {
        var story = await _storageService.GetStoryAsync(id);
        if (story == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy truyện";
            return RedirectToAction("Index");
        }

        var userId = GetCurrentUserId();
        var isOwner = story.UserId.HasValue && story.UserId == userId;

        var viewModel = new StoryDetailsViewModel
        {
            Story = story,
            CurrentPage = Math.Clamp(page, 1, story.Pages.Count),
            IsOwner = isOwner,
            ShareUrl = story.ShareToken != null 
                ? $"{Request.Scheme}://{Request.Host}/Story/Shared/{story.ShareToken}" 
                : null
        };

        return View(viewModel);
    }

    // GET: /Story/Shared/{shareToken}
    public async Task<IActionResult> Shared(string shareToken, int page = 1)
    {
        var story = await _storageService.GetStoryByShareTokenAsync(shareToken);
        if (story == null)
        {
            TempData["ErrorMessage"] = "Link chia sẻ không hợp lệ hoặc đã hết hạn";
            return RedirectToAction("Index", "Home");
        }

        var viewModel = new StoryDetailsViewModel
        {
            Story = story,
            CurrentPage = Math.Clamp(page, 1, story.Pages.Count),
            IsOwner = false
        };

        return View("SharedDetails", viewModel);
    }

    // POST: /Story/ToggleFavorite/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var isFavorite = await _storageService.ToggleFavoriteAsync(id);
        TempData["SuccessMessage"] = isFavorite ? "Đã thêm vào yêu thích" : "Đã bỏ yêu thích";
        return RedirectToAction("Details", new { id });
    }

    // POST: /Story/GenerateShareLink/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateShareLink(Guid id)
    {
        try
        {
            var shareToken = await _storageService.GenerateShareTokenAsync(id);
            var shareUrl = $"{Request.Scheme}://{Request.Host}/Story/Shared/{shareToken}";
            TempData["ShareUrl"] = shareUrl;
            TempData["SuccessMessage"] = "Đã tạo link chia sẻ!";
        }
        catch
        {
            TempData["ErrorMessage"] = "Không thể tạo link chia sẻ";
        }
        return RedirectToAction("Details", new { id });
    }

    // GET: /Story/Edit/{id}
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        var story = await _storageService.GetStoryAsync(id);
        if (story == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        if (story.UserId != userId)
        {
            return Forbid();
        }

        var viewModel = new EditStoryViewModel
        {
            Id = story.Id,
            Title = story.Title,
            Description = story.Description,
            Theme = story.Theme
        };

        return View(viewModel);
    }

    // POST: /Story/Edit/{id}
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditStoryViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var story = await _storageService.GetStoryAsync(id);
        if (story == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        if (story.UserId != userId)
        {
            return Forbid();
        }

        story.Title = model.Title;
        story.Description = model.Description ?? "";
        story.Theme = model.Theme ?? "";
        story.UpdatedAt = DateTime.UtcNow;

        await _storageService.SaveStoryAsync(story);

        TempData["SuccessMessage"] = "Đã cập nhật truyện!";
        return RedirectToAction("Details", new { id });
    }

    // POST: /Story/Delete/{id}
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var story = await _storageService.GetStoryAsync(id);
        if (story == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy truyện";
            return RedirectToAction("Index");
        }

        var userId = GetCurrentUserId();
        if (story.UserId != userId)
        {
            return Forbid();
        }

        await _storageService.DeleteStoryAsync(id);
        TempData["SuccessMessage"] = "Đã xóa truyện!";
        return RedirectToAction("Index");
    }

    // POST: /Story/RegenerateImage/{id}/{pageNumber}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegenerateImage(Guid id, int pageNumber)
    {
        try
        {
            var story = await _storageService.GetStoryAsync(id);
            if (story == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy truyện";
                return RedirectToAction("Index");
            }

            var page = story.Pages.FirstOrDefault(p => p.PageNumber == pageNumber);
            if (page == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy trang";
                return RedirectToAction("Details", new { id });
            }

            var imagePrompt = _imagePromptService.BuildImagePrompt(story, page);
            var newImageUrl = await _imageService.GenerateImageBase64(imagePrompt);

            await _storageService.UpdateStoryPageAsync(id, pageNumber, imageUrl: newImageUrl);

            TempData["SuccessMessage"] = "Đã tạo lại hình ảnh!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating image");
            TempData["ErrorMessage"] = "Lỗi khi tạo lại hình ảnh";
        }

        return RedirectToAction("Details", new { id, page = pageNumber });
    }
}
