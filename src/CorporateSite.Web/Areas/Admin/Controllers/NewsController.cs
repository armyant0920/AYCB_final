using CorporateSite.Application.Dtos;
using CorporateSite.Application.Services;
using CorporateSite.Domain.Enums;
using CorporateSite.Web.ViewModels.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Editor")]
public class NewsController : Controller
{
    private readonly NewsService _newsService;
    public NewsController(NewsService newsService) => _newsService = newsService;

    public IActionResult Index(int page = 1)
    {
        var result = _newsService.GetList(page, pageSize: 20);
        return View(result);
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null) return View(new NewsEditViewModel());

        var news = _newsService.GetById(id.Value);
        if (news == null) return NotFound();

        return View(new NewsEditViewModel
        {
            NewsId = news.NewsId,
            Title = news.Title,
            Summary = news.Summary,
            BodyHtml = news.BodyHtml,
            Category = news.Category,
            Publish = news.Status == PublishStatus.Published,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(NewsEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var dto = new NewsEditDto
        {
            Title = vm.Title,
            Summary = vm.Summary,
            BodyHtml = vm.BodyHtml,
            Category = vm.Category,
            Publish = vm.Publish,
        };

        var userName = User.Identity?.Name ?? "system";
        if (vm.NewsId == null)
            _newsService.Create(dto, userName);
        else
            _newsService.Update(vm.NewsId.Value, dto);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _newsService.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
