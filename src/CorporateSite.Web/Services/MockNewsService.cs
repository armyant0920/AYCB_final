using CorporateSite.Web.Infrastructure;
using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

/// <summary>
/// 開發用 mock — 設定 UseMockData:true 時自動啟用，不需要 DB 或 Api。
/// </summary>
public class MockNewsService : INewsService
{
    private static int _nextId = 100;
    private static readonly List<NewsDto> _store =
    [
        new() { NewsId=1, Title="公司成立30週年慶典", Summary="感謝各界長期支持，公司持續茁壯成長。",
                BodyHtml="<p>感謝各界長期支持，我們將持續努力，為客戶與社會創造更大價值。</p>",
                Category="CompanyNews", Language="zh-TW", Status=1,
                PublishedAt=DateTime.UtcNow.AddDays(-10) },
        new() { NewsId=2, Title="2024 ESG 永續報告書發佈", Summary="本年度 ESG 報告正式對外公告。",
                BodyHtml="<p>本公司致力永續發展，詳細內容請見附件。</p>",
                Category="ESG", Language="zh-TW", Status=1,
                PublishedAt=DateTime.UtcNow.AddDays(-5) },
        new() { NewsId=3, Title="Annual Sustainability Report 2024", Summary="Our ESG report is now available.",
                BodyHtml="<p>We are committed to sustainable development.</p>",
                Category="ESG", Language="en-US", Status=1,
                PublishedAt=DateTime.UtcNow.AddDays(-5) },
        new() { NewsId=4, Title="新品發表預告（草稿）", Summary="即將推出全新產品線。",
                BodyHtml="<p>草稿內容，Status=0。</p>",
                Category="CompanyNews", Language="zh-TW", Status=0, PublishedAt=null },
    ];

    public Task<List<NewsDto>> GetPublishedAsync(string? category = null, string? language = null, int top = 20)
    {
        var q = _store.Where(n => n.Status == 1);
        if (category != null) q = q.Where(n => n.Category == category);
        if (language != null) q = q.Where(n => n.Language == language);
        return Task.FromResult(q.OrderByDescending(n => n.PublishedAt).Take(top).ToList());
    }

    public Task<NewsDto?> GetByIdAsync(int id)
        => Task.FromResult(_store.FirstOrDefault(n => n.NewsId == id));

    public Task<PagedResult<NewsDto>> GetListAsync(int page = 1, int pageSize = 20,
        string? category = null, string? language = null, int? status = null)
    {
        var q = _store.AsQueryable();
        if (category != null) q = q.Where(n => n.Category == category);
        if (language != null) q = q.Where(n => n.Language == language);
        if (status.HasValue) q = q.Where(n => n.Status == status.Value);

        var total = q.Count();
        var items = q.OrderByDescending(n => n.PublishedAt ?? n.NewsId)
                     .Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedResult<NewsDto>
        {
            Items = items, TotalCount = total, Page = page, PageSize = pageSize
        });
    }

    public Task<(bool ok, string error)> CreateAsync(NewsEditRequest req)
    {
        _store.Add(new NewsDto
        {
            NewsId = ++_nextId,
            Title = req.Title.Trim(),
            Summary = req.Summary?.Trim() ?? "",
            BodyHtml = BasicHtmlSanitizer.Sanitize(req.BodyHtml),
            Category = req.Category,
            Language = req.Language,
            Status = req.Publish ? 1 : 0,
            PublishedAt = req.Publish ? DateTime.UtcNow : null,
        });
        return Task.FromResult((true, ""));
    }

    public Task<(bool ok, string error)> UpdateAsync(int id, NewsEditRequest req)
    {
        var item = _store.FirstOrDefault(n => n.NewsId == id);
        if (item == null) return Task.FromResult((false, "找不到資料"));

        item.Title = req.Title.Trim();
        item.Summary = req.Summary?.Trim() ?? "";
        item.BodyHtml = BasicHtmlSanitizer.Sanitize(req.BodyHtml);
        item.Category = req.Category;
        item.Language = req.Language;
        item.Status = req.Publish ? 1 : 0;
        item.PublishedAt = req.Publish ? (item.PublishedAt ?? DateTime.UtcNow) : null;
        return Task.FromResult((true, ""));
    }

    public Task<(bool ok, string error)> DeleteAsync(int id)
    {
        var item = _store.FirstOrDefault(n => n.NewsId == id);
        if (item != null) _store.Remove(item);
        return Task.FromResult((true, ""));
    }
}
