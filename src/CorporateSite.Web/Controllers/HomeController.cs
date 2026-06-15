using CorporateSite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Controllers;

public class HomeController : Controller
{
    private readonly INewsService _news;
    public HomeController(INewsService news) => _news = news;

    public async Task<IActionResult> Index()
    {
        var latest = await _news.GetPublishedAsync(top: 3);
        return View(latest);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
