using CorporateSite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Controllers;

public class ArticleController : Controller
{
    private readonly IArticleService _api;
    public ArticleController(IArticleService api) => _api = api;

    public async Task<IActionResult> Index(string? section)
    {
        var articles = await _api.GetPublishedAsync(section);
        ViewBag.Section = section;
        return View(articles);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var article = await _api.GetBySlugAsync(slug);
        if (article == null) return NotFound();
        return View(article);
    }
}
