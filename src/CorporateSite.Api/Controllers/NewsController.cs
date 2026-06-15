using CorporateSite.Api.Models;
using CorporateSite.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly NewsRepository _repo;

    public NewsController(NewsRepository repo) => _repo = repo;

    // TODO: 取得 HtmlSanitizer 正確套件 ID 後，將此方法換成真正的淨化實作
    private static string SanitizeHtml(string html) => html;

    [HttpGet("published")]
    public IActionResult GetPublished([FromQuery] string? category, [FromQuery] int top = 20)
        => Ok(_repo.GetPublished(category, top));

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var news = _repo.GetById(id);
        return news == null ? NotFound() : Ok(news);
    }

    [HttpGet]
    public IActionResult GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null, [FromQuery] int? status = null)
        => Ok(_repo.GetList(page, pageSize, category, status));

    [HttpPost]
    public IActionResult Create([FromBody] NewsEditRequest req)
    {
        var news = new News
        {
            Title = req.Title.Trim(),
            Summary = req.Summary?.Trim() ?? "",
            BodyHtml = SanitizeHtml(req.BodyHtml),
            Category = req.Category,
            Status = req.Publish ? 1 : 0,
            PublishedAt = req.Publish ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = req.CreatedBy,
        };
        int id = _repo.Insert(news);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] NewsEditRequest req)
    {
        var existing = _repo.GetById(id);
        if (existing == null) return NotFound();

        existing.Title = req.Title.Trim();
        existing.Summary = req.Summary?.Trim() ?? "";
        existing.BodyHtml = SanitizeHtml(req.BodyHtml);
        existing.Category = req.Category;
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
