using CorporateSite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Controllers;

public class NewsController : Controller
{
    private readonly INewsService _api;
    public NewsController(INewsService api) => _api = api;

    public async Task<IActionResult> Index(string? category)
    {
        var news = await _api.GetPublishedAsync(category);
        ViewBag.Category = category;
        return View(news);
    }

    public async Task<IActionResult> Details(int id)
    {
        var news = await _api.GetByIdAsync(id);
        if (news == null) return NotFound();
        return View(news);
    }
}
