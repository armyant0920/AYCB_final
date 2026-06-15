using CorporateSite.Web.Models;
using CorporateSite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Controllers;

public class NewsController : Controller
{
    private readonly INewsService _api;
    public NewsController(INewsService api) => _api = api;

    public async Task<IActionResult> Index(string? category, int page = 1)
    {
        const int PageSize = 9;
        var result = await _api.GetListAsync(page, PageSize, category: category, status: 1);
        ViewBag.Category = category;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var news = await _api.GetByIdAsync(id);
        if (news == null) return NotFound();
        return View(news);
    }
}

