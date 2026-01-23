using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Server.Services;

public class ImageGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImageGenerationService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string? _googleToken;

    private readonly int _retries;
    public ImageGenerationService(HttpClient httpClient, ILogger<ImageGenerationService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["WhomeAI:ApiKey"] ?? "sk-demo";
        _baseUrl = configuration["WhomeAI:BaseUrl"] ?? "https://api.whomeai.com";
        _googleToken = configuration["WhomeAI:GoogleToken"];
        _retries = int.TryParse(configuration["WhomeAI:Retries"], out var r) ? Math.Max(1, r) : 5;
    }

    public async Task<string> GenerateImageBase64(string prompt, string size = "1792x1024", string model = "gemini-2.5-flash")
    {
        // Returns data URL string: data:image/png;base64,<b64>
        var url = $"{_baseUrl}/v1/images/generations";
        var requestBody = new
        {
            model = model,
            prompt = prompt,
            n = 1,
            size = size,
            response_format = "b64_json"
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Configurable retry attempts with simple backoff
        var attempts = 0;
        Exception? lastError = null;
        while (attempts < _retries)
        {
            attempts++;
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                
                // Thêm Google Token nếu có (cho Gemini Image API)
                if (!string.IsNullOrEmpty(_googleToken))
                {
                    request.Headers.Add("X-Google-Token", _googleToken);
                }

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"WhomeAI API error (attempt {attempts}/{_retries}): {response.StatusCode} - {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
                    throw new Exception($"WhomeAI API returned error: {response.StatusCode} - {responseContent.Substring(0, Math.Min(100, responseContent.Length))}");
                }

                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (!root.TryGetProperty("data", out var dataArray) || dataArray.GetArrayLength() == 0)
                {
                    throw new Exception("WhomeAI response missing data");
                }
                var b64 = dataArray[0].GetProperty("b64_json").GetString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(b64))
                {
                    _logger.LogError("WhomeAI response missing b64_json. Full response: " + responseContent.Substring(0, Math.Min(500, responseContent.Length)));
                    throw new Exception("WhomeAI response missing b64_json");
                }
                
                var dataUrl = $"data:image/png;base64,{b64}";
                _logger.LogInformation($"✅ Successfully received image data ({b64.Length} chars base64)");
                return dataUrl;
            }
            catch (Exception ex)
            {
                lastError = ex;
                _logger.LogWarning($"Image generation attempt {attempts} failed: {ex.Message}");
                if (attempts < _retries)
                {
                    var delayMs = 500 * attempts; // Progressive backoff: 500ms, 1000ms, 1500ms...
                    _logger.LogInformation($"Retrying after {delayMs}ms...");
                    await Task.Delay(delayMs);
                }
            }
        }

        _logger.LogError(lastError, "WhomeAI failed after retries");
        throw new Exception("WhomeAI failed after retries", lastError);
    }
}


