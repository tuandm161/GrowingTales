using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Server.Models;

namespace Server.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiService> _logger;
    private readonly string _apiKey;
    private readonly ImageGenerationService _imageService;
    private readonly ImagePromptService _imagePromptService;
    private readonly int _imageDelayMs;

    public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, IConfiguration configuration, ImageGenerationService imageService, ImagePromptService imagePromptService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("Gemini API key is missing. Please set 'Gemini:ApiKey' in configuration.");
        }
        _imageService = imageService;
        _imagePromptService = imagePromptService;
        _imageDelayMs = int.TryParse(configuration["WhomeAI:DelayBetweenPagesMs"], out var d) ? Math.Max(0, d) : 300;
    }

    public async Task<Story> GenerateStoryFromText(StoryRequest request)
    {
        var prompt = BuildStoryPrompt(request);
        var generatedContent = await CallGeminiApi(prompt);
        
        var story = ParseStoryFromResponse(generatedContent, request);
        
        // Generate images for each page using separate image prompt (cute, wholesome, vibrant cartoon)
        _logger.LogInformation($"Starting image generation for {story.Pages.Count} pages");
        
        for (int i = 0; i < story.Pages.Count; i++)
        {
            var page = story.Pages[i];
            _logger.LogInformation($"Generating image for page {page.PageNumber} ({i + 1}/{story.Pages.Count})");
            
            bool imageGenerated = false;
            
            try
            {
                var imagePrompt = _imagePromptService.BuildImagePrompt(story, page);
                _logger.LogInformation($"Image prompt for page {page.PageNumber}: {imagePrompt.Substring(0, Math.Min(50, imagePrompt.Length))}...");
                
                var dataUrl = await _imageService.GenerateImageBase64(imagePrompt);
                
                if (!string.IsNullOrWhiteSpace(dataUrl) && dataUrl.StartsWith("data:image"))
                {
                    page.ImageUrl = dataUrl;
                    imageGenerated = true;
                    _logger.LogInformation($" Successfully generated image for page {page.PageNumber}");
                }
                else
                {
                    _logger.LogWarning($" Invalid image data URL for page {page.PageNumber}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Failed to generate image for page {page.PageNumber}: {ex.Message}");
            }
            
            // Always set fallback if image generation failed
            if (!imageGenerated || string.IsNullOrWhiteSpace(page.ImageUrl))
            {
                try
                {
                    // Try to use local fallback image
                    var localImagePath = @"C:\Users\tuann\Downloads\123.jpg";
                    if (System.IO.File.Exists(localImagePath))
                    {
                        var imageBytes = await System.IO.File.ReadAllBytesAsync(localImagePath);
                        var base64 = Convert.ToBase64String(imageBytes);
                        page.ImageUrl = $"data:image/jpeg;base64,{base64}";
                        _logger.LogWarning($"🔄 Using local fallback image for page {page.PageNumber}");
                    }
                    else
                    {
                        _logger.LogWarning($" Local file not found for page {page.PageNumber}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load local fallback image for page {page.PageNumber}");
                }
            }
            
            // Delay between pages to avoid rate limiting (except for last page)
            if (i < story.Pages.Count - 1 && _imageDelayMs > 0)
            {
                _logger.LogInformation($"Waiting {_imageDelayMs}ms before next image generation...");
                await Task.Delay(_imageDelayMs);
            }
        }

        // Final validation: ensure every page has non-empty ImageUrl
        _logger.LogInformation("Validating all pages have images...");
        int missingCount = 0;
        foreach (var page in story.Pages)
        {
            if (string.IsNullOrWhiteSpace(page.ImageUrl))
            {
                missingCount++;
                try
                {
                    var localImagePath = @"C:\Users\tuann\Downloads\123.jpg";
                    if (System.IO.File.Exists(localImagePath))
                    {
                        var imageBytes = await System.IO.File.ReadAllBytesAsync(localImagePath);
                        var base64 = Convert.ToBase64String(imageBytes);
                        page.ImageUrl = $"data:image/jpeg;base64,{base64}";
                        _logger.LogWarning($" Fixed missing image for page {page.PageNumber}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load local image for page {page.PageNumber}");
                }
            }
        }
        
        if (missingCount > 0)
        {
            _logger.LogWarning($"Fixed {missingCount} pages with missing images using placeholders");
        }
        else
        {
            _logger.LogInformation(" All pages have images assigned");
        }
        
        return story;
    }

    public async Task<Story> GenerateStoryFromAudio(AudioStoryRequest request)
    {
        // First, transcribe audio to text using Gemini
        var transcribedText = await TranscribeAudio(request.AudioBase64);

        // Then generate story from the transcribed text
        var storyRequest = new StoryRequest
        {
            InputText = transcribedText,
            ChildName = request.ChildName,
            ChildAge = request.ChildAge,
            Theme = request.Theme,
            PageCount = request.PageCount,
            Language = request.Language
        };

        var story = await GenerateStoryFromText(storyRequest);
        
        // Images are already generated in GenerateStoryFromText
        return story;
    }

    private async Task<string> TranscribeAudio(string audioBase64)
    {
        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = "Hãy phiên âm (transcribe) đoạn audio này thành văn bản tiếng Việt. Chỉ trả về nội dung văn bản, không thêm bất kỳ giải thích nào." },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "audio/wav",
                                    data = audioBase64
                                }
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Gemini API error: {responseContent}");
                throw new Exception($"Failed to transcribe audio: {response.StatusCode}");
            }

            var result = JsonDocument.Parse(responseContent);
            var text = result.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return text.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transcribing audio");
            throw;
        }
    }

    private string BuildStoryPrompt(StoryRequest request)
    {
        var prompt = $@"Bạn là một nhà văn chuyên viết truyện cho trẻ em. Hãy tạo một câu chuyện đẹp và ý nghĩa, giàu chi tiết, dựa trên thông tin sau:

Tên bé: {request.ChildName}
Tuổi: {request.ChildAge}
Chủ đề: {request.Theme}
Nội dung gợi ý: {request.InputText}
Bối cảnh thêm: {request.AdditionalContext}
Số trang: {request.PageCount}

Yêu cầu:
1. Tạo một câu chuyện phù hợp với lứa tuổi {request.ChildAge}
2. Câu chuyện phải có ý nghĩa giáo dục, truyền cảm hứng tích cực
3. Sử dụng ngôn ngữ đơn giản, dễ hiểu, ấm áp
4. Chia thành {request.PageCount} trang, mỗi trang khoảng 4-6 câu văn.
   - Ưu tiên thêm miêu tả cảm xúc, âm thanh, màu sắc, và 1-2 câu hội thoại ngắn
   - Duy trì mạch truyện mượt mà, có mở đầu, cao trào và kết thúc dịu nhẹ
5. Mỗi trang cần có mô tả hình ảnh (imagePrompt) để minh họa

⚠️ QUAN TRỌNG:
- ""content"" phải viết bằng TIẾNG VIỆT
- ""imagePrompt"" BẮT BUỘC phải viết bằng TIẾNG ANH (English)
- imagePrompt phải ngắn gọn, chỉ 3-5 từ khóa chính bằng tiếng Anh

Trả về kết quả theo format JSON sau:
{{
  ""title"": ""Tiêu đề câu chuyện (tiếng Việt)"",
  ""description"": ""Mô tả ngắn về câu chuyện (tiếng Việt)"",
  ""pages"": [
    {{
      ""pageNumber"": 1,
      ""content"": ""Nội dung trang 1 (tiếng Việt)"",
      ""imagePrompt"": ""happy child playing garden (English keywords only)""
    }}
  ]
}}

Chỉ trả về JSON, không thêm bất kỳ text nào khác.";

        return prompt;
    }

    private async Task<string> CallGeminiApi(string prompt)
    {
        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 8192,
                    responseMimeType = "application/json"
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Calling Gemini API...");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Gemini API error: {responseContent}");
                throw new Exception($"Gemini API returned error: {response.StatusCode}");
            }

            var result = JsonDocument.Parse(responseContent);
            var text = result.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API");
            throw;
        }
    }

    private Story ParseStoryFromResponse(string jsonResponse, StoryRequest request)
    {
        try
        {
            _logger.LogInformation($"Raw response from Gemini: {jsonResponse}");

            // Remove markdown code blocks if present
            jsonResponse = jsonResponse.Trim();
            if (jsonResponse.StartsWith("```json"))
            {
                jsonResponse = jsonResponse.Substring(7);
            }
            if (jsonResponse.StartsWith("```"))
            {
                jsonResponse = jsonResponse.Substring(3);
            }
            if (jsonResponse.EndsWith("```"))
            {
                jsonResponse = jsonResponse.Substring(0, jsonResponse.Length - 3);
            }
            jsonResponse = jsonResponse.Trim();

            _logger.LogInformation($"Cleaned JSON: {jsonResponse}");

            var storyData = JsonDocument.Parse(jsonResponse);
            var root = storyData.RootElement;

            // Safely extract properties
            string title = "Untitled Story";
            if (root.TryGetProperty("title", out var titleElement))
            {
                title = titleElement.GetString() ?? "Untitled Story";
            }

            string description = "";
            if (root.TryGetProperty("description", out var descElement))
            {
                description = descElement.GetString() ?? "";
            }

            var story = new Story
            {
                Title = title,
                Description = description,
                ChildName = request.ChildName,
                ChildAge = request.ChildAge,
                Theme = request.Theme,
                Pages = new List<StoryPage>()
            };

            // Check if pages array exists
            if (!root.TryGetProperty("pages", out var pagesArray))
            {
                _logger.LogError($"Response does not contain 'pages' property. Full response: {jsonResponse}");
                throw new Exception("Response từ Gemini không có trường 'pages'. Có thể API key không hợp lệ hoặc format response bị thay đổi.");
            }

            int pageNumber = 1;
            foreach (var page in pagesArray.EnumerateArray())
            {
                var storyPage = new StoryPage
                {
                    PageNumber = pageNumber++,
                    Content = page.TryGetProperty("content", out var contentElement) 
                        ? contentElement.GetString() ?? "" 
                        : "",
                    ImagePrompt = page.TryGetProperty("imagePrompt", out var promptElement) 
                        ? promptElement.GetString() ?? "" 
                        : ""
                };
                story.Pages.Add(storyPage);
            }

            if (story.Pages.Count == 0)
            {
                _logger.LogWarning("No pages were parsed from the response");
                throw new Exception("Không tạo được trang truyện nào từ response");
            }

            return story;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"JSON parsing error. Response: {jsonResponse}");
            throw new Exception($"Lỗi parse JSON từ Gemini: {ex.Message}. Response có thể không đúng format.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error parsing story response: {jsonResponse}");
            throw new Exception($"Lỗi xử lý response: {ex.Message}", ex);
        }
    }

}

