using CorporateSite.Api.Infrastructure;
using CorporateSite.Api.Models;
using CorporateSite.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly ArticleRepository _repo;
    public ArticleController(ArticleRepository repo) => _repo = repo;

    [HttpGet("published")]
    public IActionResult GetPublished([FromQuery] string? section, [FromQuery] string? language)
        => Ok(_repo.GetPublished(section, language));

    [HttpGet("by-slug/{slug}")]
    public IActionResult GetBySlug(string slug)
    {
        var article = _repo.GetBySlug(slug);
        return article == null ? NotFound() : Ok(article);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var article = _repo.GetById(id);
        return article == null ? NotFound() : Ok(article);
    }

    [HttpGet]
    public IActionResult GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? section = null, [FromQuery] string? language = null,
        [FromQuery] int? status = null)
        => Ok(_repo.GetList(page, pageSize, section, language, status));

    [HttpPost]
    public IActionResult Create([FromBody] ArticleEditRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest(new { error = "標題為必填" });
        if (string.IsNullOrWhiteSpace(req.Slug))
            return BadRequest(new { error = "Slug 為必填" });

        var article = new Article
        {
            Slug = req.Slug.Trim().ToLowerInvariant(),
            Title = req.Title.Trim(),
            Summary = req.Summary?.Trim() ?? "",
            BodyHtml = BasicHtmlSanitizer.Sanitize(req.BodyHtml),
            Section = req.Section,
            Language = req.Language,
            SortOrder = req.SortOrder,
            Status = req.Publish ? 1 : 0,
            PublishedAt = req.Publish ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = req.CreatedBy,
        };
        int id = _repo.Insert(article);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ArticleEditRequest req)
    {
        var existing = _repo.GetById(id);
        if (existing == null) return NotFound();

        existing.Slug = req.Slug.Trim().ToLowerInvariant();
        existing.Title = req.Title.Trim();
        existing.Summary = req.Summary?.Trim() ?? "";
        existing.BodyHtml = BasicHtmlSanitizer.Sanitize(req.BodyHtml);
        existing.Section = req.Section;
        existing.Language = req.Language;
        existing.SortOrder = req.SortOrder;
        existing.Status = req.Publish ? 1 : 0;
        existing.PublishedAt = req.Publish ? (existing.PublishedAt ?? DateTime.UtcNow) : null;
        existing.UpdatedAt = DateTime.UtcNow;

        _repo.Update(existing);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _repo.SoftDelete(id);
        return NoContent();
    }
}
