using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

public interface IArticleService
{
    Task<List<ArticleDto>> GetPublishedAsync(string? section = null, string? language = null);
    Task<ArticleDto?> GetBySlugAsync(string slug);
    Task<ArticleDto?> GetByIdAsync(int id);
    Task<PagedResult<ArticleDto>> GetListAsync(int page = 1, int pageSize = 20,
        string? section = null, string? language = null, int? status = null);
    Task<(bool ok, string error)> CreateAsync(ArticleEditRequest req);
    Task<(bool ok, string error)> UpdateAsync(int id, ArticleEditRequest req);
    Task<(bool ok, string error)> DeleteAsync(int id);
}

public class ArticleEditRequest
{
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Section { get; set; } = "";
    public string Language { get; set; } = "zh-TW";
    public int SortOrder { get; set; }
    public bool Publish { get; set; }
    public string CreatedBy { get; set; } = "";
}
