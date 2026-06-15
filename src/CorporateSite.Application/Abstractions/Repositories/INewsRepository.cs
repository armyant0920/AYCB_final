using CorporateSite.Domain.Common;
using CorporateSite.Domain.Entities;

namespace CorporateSite.Application.Abstractions.Repositories;

public interface INewsRepository
{
    PagedResult<News> GetList(int page, int pageSize, string? category = null, int? status = null);
    IReadOnlyList<News> GetPublished(string? category, int top);
    News? GetById(int newsId);
    int Insert(News news);
    int Update(News news);
    int SoftDelete(int newsId);
}
