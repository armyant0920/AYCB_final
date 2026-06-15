using CorporateSite.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Controllers;

public class NewsController : Controller
{
    private readonly NewsService _newsService;
    public NewsController(NewsService newsService) => _newsService = newsService;

    public IActionResult Index(string? category)
    {
        var news = _newsService.GetPublished(category, top: 20);
        ViewBag.Category = category;
        return View(news);
    }

    public IActionResult Details(int id)
    {
        var news = _newsService.GetById(id);
        if (news == null) return NotFound();
        return View(news);
    }
}
