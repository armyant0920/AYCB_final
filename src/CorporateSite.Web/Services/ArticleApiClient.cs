using System.Net.Http.Json;
using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

public class ArticleApiClient : IArticleService
{
    private readonly HttpClient _http;
    private readonly ILogger<ArticleApiClient> _logger;

    public ArticleApiClient(HttpClient http, ILogger<ArticleApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<ArticleDto>> GetPublishedAsync(string? section = null, string? language = null)
    {
        try
        {
            var url = "/api/article/published";
            var qs = new List<string>();
            if (section  != null) qs.Add($"section={Uri.EscapeDataString(section)}");
            if (language != null) qs.Add($"language={Uri.EscapeDataString(language)}");
            if (qs.Count > 0) url += "?" + string.Join("&", qs);
            return await _http.GetFromJsonAsync<List<ArticleDto>>(url) ?? [];
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Api unavailable: Article.GetPublished"); return []; }
    }

    public async Task<ArticleDto?> GetBySlugAsync(string slug)
    {
        try { return await _http.GetFromJsonAsync<ArticleDto>($"/api/article/by-slug/{Uri.EscapeDataString(slug)}"); }
        catch (Exception ex) { _logger.LogWarning(ex, "Api unavailable: Article.GetBySlug {Slug}", slug); return null; }
    }

    public async Task<ArticleDto?> GetByIdAsync(int id)
    {
        try { return await _http.GetFromJsonAsync<ArticleDto>($"/api/article/{id}"); }
        catch (Exception ex) { _logger.LogWarning(ex, "Api unavailable: Article.GetById {Id}", id); return null; }
    }

    public async Task<PagedResult<ArticleDto>> GetListAsync(int page = 1, int pageSize = 20,
        string? section = null, string? language = null, int? status = null)
    {
        try
        {
            var url = $"/api/article?page={page}&pageSize={pageSize}";
            if (section  != null) url += $"&section={Uri.EscapeDataString(section)}";
            if (language != null) url += $"&language={Uri.EscapeDataString(language)}";
            if (status.HasValue)  url += $"&status={status}";
            return await _http.GetFromJsonAsync<PagedResult<ArticleDto>>(url) ?? new();
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Api unavailable: Article.GetList"); return new(); }
    }

    public async Task<(bool ok, string error)> CreateAsync(ArticleEditRequest req)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/article", req);
            return resp.IsSuccessStatusCode ? (true, "") : (false, $"API 回應 {(int)resp.StatusCode}");
        }
        catch (Exception ex) { _logger.LogError(ex, "Article.CreateAsync failed"); return (false, "無法連線至 API 伺服器"); }
    }

    public async Task<(bool ok, string error)> UpdateAsync(int id, ArticleEditRequest req)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"/api/article/{id}", req);
            return resp.IsSuccessStatusCode ? (true, "") : (false, $"API 回應 {(int)resp.StatusCode}");
        }
        catch (Exception ex) { _logger.LogError(ex, "Article.UpdateAsync failed"); return (false, "無法連線至 API 伺服器"); }
    }

    public async Task<(bool ok, string error)> DeleteAsync(int id)
    {
        try
        {
            var resp = await _http.DeleteAsync($"/api/article/{id}");
            return resp.IsSuccessStatusCode ? (true, "") : (false, $"API 回應 {(int)resp.StatusCode}");
        }
        catch (Exception ex) { _logger.LogError(ex, "Article.DeleteAsync failed"); return (false, "無法連線至 API 伺服器"); }
    }
}
