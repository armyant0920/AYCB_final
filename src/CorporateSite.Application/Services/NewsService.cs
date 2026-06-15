using CorporateSite.Application.Abstractions.Content;
using CorporateSite.Application.Abstractions.Repositories;
using CorporateSite.Application.Dtos;
using CorporateSite.Domain.Common;
using CorporateSite.Domain.Entities;
using CorporateSite.Domain.Enums;

namespace CorporateSite.Application.Services;

public class NewsService
{
    private readonly INewsRepository _repo;
    private readonly IHtmlSanitizerService _sanitizer;

    public NewsService(INewsRepository repo, IHtmlSanitizerService sanitizer)
    {
        _repo = repo;
        _sanitizer = sanitizer;
    }

    public IReadOnlyList<News> GetPublished(string? category = null, int top = 10)
        => _repo.GetPublished(category, top);

    public News? GetById(int id) => _repo.GetById(id);

    public PagedResult<News> GetList(int page, int pageSize, string? category = null, int? status = null)
        => _repo.GetList(page, pageSize, category, status);

    public int Create(NewsEditDto dto, string createdBy)
    {
        var news = new News
        {
            Title = dto.Title.Trim(),
            Summary = dto.Summary?.Trim() ?? "",
            BodyHtml = _sanitizer.Sanitize(dto.BodyHtml),
            Category = dto.Category,
            Status = dto.Publish ? PublishStatus.Published : PublishStatus.Draft,
            PublishedAt = dto.Publish ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
        };
        return _repo.Insert(news);
    }

    public int Update(int newsId, NewsEditDto dto)
    {
        var existing = _repo.GetById(newsId)
            ?? throw new KeyNotFoundException($"News {newsId} not found.");

        existing.Title = dto.Title.Trim();
        existing.Summary = dto.Summary?.Trim() ?? "";
        existing.BodyHtml = _sanitizer.Sanitize(dto.BodyHtml);
        existing.Category = dto.Category;
        existing.Status = dto.Publish ? PublishStatus.Published : PublishStatus.Draft;
        existing.PublishedAt = dto.Publish ? (existing.PublishedAt ?? DateTime.UtcNow) : null;
        existing.UpdatedAt = DateTime.UtcNow;

        return _repo.Update(existing);
    }

    public int Delete(int newsId) => _repo.SoftDelete(newsId);
}
