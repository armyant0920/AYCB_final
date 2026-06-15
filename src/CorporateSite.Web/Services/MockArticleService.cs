using CorporateSite.Web.Infrastructure;
using CorporateSite.Web.Models;

namespace CorporateSite.Web.Services;

public class MockArticleService : IArticleService
{
    private static int _nextId = 100;
    private static readonly List<ArticleDto> _store =
    [
        new() { ArticleId=1, Slug="about-us", Title="公司簡介",
                Summary="專注於高端電路板技術，服務全球半導體產業。",
                BodyHtml="<p>本公司成立於1990年代，專注於高端電路板技術，持續創新，服務全球半導體測試產業。</p><p>我們的核心技術包括高頻高速解決方案、探針卡設計與製造，以及完整的 IC 測試板服務。</p>",
                Section="About", Language="zh-TW", SortOrder=1, Status=1,
                PublishedAt=DateTime.UtcNow.AddDays(-100) },
        new() { ArticleId=2, Slug="vision-mission", Title="願景與使命",
                Summary="以技術創新驅動永續成長，成為全球最受信賴的電路板解決方案夥伴。",
                BodyHtml="<h2>願景</h2><p>成為全球最受信賴的電路板解決方案夥伴。</p><h2>使命</h2><p>以技術創新驅動永續成長，為客戶、員工與社會創造長期價值。</p>",
                Section="About", Language="zh-TW", SortOrder=2, Status=1,
                PublishedAt=DateTime.UtcNow.AddDays(-90) },
        new() { ArticleId=3, Slug="esg-policy", Title="ESG 永續政策",
                Summary="落實環境保護、社會責任與公司治理，建立永續企業。",
                BodyHtml="<p>本公司承諾在2030年前達成碳中和目標，並持續提升供應鏈透明度與員工福祉。</p>",
                Section="ESG", Language="zh-TW", SortOrder=1, Status=1,
                PublishedAt=DateTime.UtcNow.AddDays(-60) },
    ];

    public Task<List<ArticleDto>> GetPublishedAsync(string? section = null, string? language = null)
    {
        var q = _store.Where(a => a.Status == 1);
        if (section != null)  q = q.Where(a => a.Section  == section);
        if (language != null) q = q.Where(a => a.Language == language);
        return Task.FromResult(q.OrderBy(a => a.SortOrder).ToList());
    }

    public Task<ArticleDto?> GetBySlugAsync(string slug)
        => Task.FromResult(_store.FirstOrDefault(a => a.Slug == slug && a.Status == 1));

    public Task<ArticleDto?> GetByIdAsync(int id)
        => Task.FromResult(_store.FirstOrDefault(a => a.ArticleId == id));

    public Task<PagedResult<ArticleDto>> GetListAsync(int page = 1, int pageSize = 20,
        string? section = null, string? language = null, int? status = null)
    {
        var q = _store.AsQueryable();
        if (section != null)    q = q.Where(a => a.Section  == section);
        if (language != null)   q = q.Where(a => a.Language == language);
        if (status.HasValue)    q = q.Where(a => a.Status   == status.Value);

        int total = q.Count();
        var items = q.OrderBy(a => a.SortOrder).ThenByDescending(a => a.ArticleId)
                     .Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedResult<ArticleDto>
        {
            Items = items, TotalCount = total, Page = page, PageSize = pageSize
        });
    }

    public Task<(bool ok, string error)> CreateAsync(ArticleEditRequest req)
    {
        _store.Add(new ArticleDto
        {
            ArticleId = ++_nextId,
            Slug      = req.Slug.Trim().ToLowerInvariant(),
            Title     = req.Title.Trim(),
            Summary   = req.Summary?.Trim() ?? "",
            BodyHtml  = BasicHtmlSanitizer.Sanitize(req.BodyHtml),
            Section   = req.Section,
            Language  = req.Language,
            SortOrder = req.SortOrder,
            Status    = req.Publish ? 1 : 0,
            PublishedAt = req.Publish ? DateTime.UtcNow : null,
        });
        return Task.FromResult((true, ""));
    }

    public Task<(bool ok, string error)> UpdateAsync(int id, ArticleEditRequest req)
    {
        var item = _store.FirstOrDefault(a => a.ArticleId == id);
        if (item == null) return Task.FromResult((false, "找不到資料"));

        item.Slug     = req.Slug.Trim().ToLowerInvariant();
        item.Title    = req.Title.Trim();
        item.Summary  = req.Summary?.Trim() ?? "";
        item.BodyHtml = BasicHtmlSanitizer.Sanitize(req.BodyHtml);
        item.Section  = req.Section;
        item.Language = req.Language;
        item.SortOrder = req.SortOrder;
        item.Status   = req.Publish ? 1 : 0;
        item.PublishedAt = req.Publish ? (item.PublishedAt ?? DateTime.UtcNow) : null;
        return Task.FromResult((true, ""));
    }

    public Task<(bool ok, string error)> DeleteAsync(int id)
    {
        var item = _store.FirstOrDefault(a => a.ArticleId == id);
        if (item != null) _store.Remove(item);
        return Task.FromResult((true, ""));
    }
}
