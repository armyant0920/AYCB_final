using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

public interface INewsService
{
    Task<List<NewsDto>> GetPublishedAsync(string? category = null, string? language = null, int top = 20);
    Task<NewsDto?> GetByIdAsync(int id);
    Task<PagedResult<NewsDto>> GetListAsync(int page = 1, int pageSize = 20,
        string? category = null, string? language = null, int? status = null);
    Task<(bool ok, string error)> CreateAsync(NewsEditRequest req);
    Task<(bool ok, string error)> UpdateAsync(int id, NewsEditRequest req);
    Task<(bool ok, string error)> DeleteAsync(int id);
}
