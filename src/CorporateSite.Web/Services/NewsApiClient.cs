using System.Net.Http.Json;
using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

public class NewsApiClient : INewsService
{
    private readonly HttpClient _http;
    private readonly ILogger<NewsApiClient> _logger;

    public NewsApiClient(HttpClient http, ILogger<NewsApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<NewsDto>> GetPublishedAsync(string? category = null, string? language = null, int top = 20)
    {
        try
        {
            var url = $"/api/news/published?top={top}";
            if (category != null) url += $"&category={Uri.EscapeDataString(category)}";
            if (language != null) url += $"&language={Uri.EscapeDataString(language)}";
            return await _http.GetFromJsonAsync<List<NewsDto>>(url) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Api unavailable: GetPublished");
            return [];
        }
    }

    public async Task<NewsDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<NewsDto>($"/api/news/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Api unavailable: GetById {Id}", id);
            return null;
        }
    }

    public async Task<PagedResult<NewsDto>> GetListAsync(int page = 1, int pageSize = 20,
        string? category = null, string? language = null, int? status = null)
    {
        try
        {
            var url = $"/api/news?page={page}&pageSize={pageSize}";
            if (category != null) url += $"&category={Uri.EscapeDataString(category)}";
            if (language != null) url += $"&language={Uri.EscapeDataString(language)}";
            if (status.HasValue)  url += $"&status={status}";
            return await _http.GetFromJsonAsync<PagedResult<NewsDto>>(url) ?? new PagedResult<NewsDto>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Api unavailable: GetList");
            return new PagedResult<NewsDto>();
        }
    }

    // 寫入操作：不靜默吞例外，由 Controller 處理錯誤訊息
    public async Task<(bool ok, string error)> CreateAsync(NewsEditRequest req)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/news", req);
            if (!resp.IsSuccessStatusCode)
                return (false, $"API 回應 {(int)resp.StatusCode}");
            return (true, "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateAsync failed");
            return (false, "無法連線至 API 伺服器");
        }
    }

    public async Task<(bool ok, string error)> UpdateAsync(int id, NewsEditRequest req)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/news/{id}", req);
            if (!resp.IsSuccessStatusCode)
                return (false, $"API 回應 {(int)resp.StatusCode}");
            return (true, "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateAsync failed");
            return (false, "無法連線至 API 伺服器");
        }
    }

    public async Task<(bool ok, string error)> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/news/{id}");
            if (!resp.IsSuccessStatusCode)
                return (false, $"API 回應 {(int)resp.StatusCode}");
            return (true, "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteAsync failed");
            return (false, "無法連線至 API 伺服器");
        }
    }
}

public class NewsEditRequest
{
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Category { get; set; } = "CompanyNews";
    public string Language { get; set; } = "zh-TW";
    public bool Publish { get; set; }
    public string CreatedBy { get; set; } = "";
}
