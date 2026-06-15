using System.Net.Http.Json;
using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

/// <summary>
/// 呼叫 CorporateSite.Api 的新聞端點。Api 未啟動時返回空結果，不拋例外。
/// </summary>
public class NewsApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<NewsApiClient> _logger;

    public NewsApiClient(HttpClient http, ILogger<NewsApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<NewsDto>> GetPublishedAsync(string? category = null, int top = 20)
    {
        try
        {
            var url = $"/api/news/published?top={top}" + (category != null ? $"&category={Uri.EscapeDataString(category)}" : "");
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

    public async Task<PagedResult<NewsDto>> GetListAsync(int page = 1, int pageSize = 20, string? category = null, int? status = null)
    {
        try
        {
            var url = $"/api/news?page={page}&pageSize={pageSize}";
            if (category != null) url += $"&category={Uri.EscapeDataString(category)}";
            if (status.HasValue) url += $"&status={status}";
            return await _http.GetFromJsonAsync<PagedResult<NewsDto>>(url) ?? new PagedResult<NewsDto>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Api unavailable: GetList");
            return new PagedResult<NewsDto>();
        }
    }

    public async Task<int> CreateAsync(NewsEditRequest req)
    {
        var resp = await _http.PostAsJsonAsync("/api/news", req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<int>();
    }

    public async Task UpdateAsync(int id, NewsEditRequest req)
    {
        var resp = await _http.PutAsJsonAsync($"/api/news/{id}", req);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/news/{id}");
        resp.EnsureSuccessStatusCode();
    }
}

public class NewsEditRequest
{
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Category { get; set; } = "CompanyNews";
    public bool Publish { get; set; }
    public string CreatedBy { get; set; } = "";
}
