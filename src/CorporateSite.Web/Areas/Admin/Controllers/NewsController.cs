using CorporateSite.Web.Services;
using CorporateSite.Web.ViewModels.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Editor")]
public class NewsController : Controller
{
    private readonly NewsApiClient _api;
    public NewsController(NewsApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _api.GetListAsync(page, pageSize: 20);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return View(new NewsEditViewModel());

        var news = await _api.GetByIdAsync(id.Value);
        if (news == null) return NotFound();

        return View(new NewsEditViewModel
        {
            NewsId = news.NewsId,
            Title = news.Title,
            Summary = news.Summary,
            BodyHtml = news.BodyHtml,
            Category = news.Category,
            Publish = news.Status == 1,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NewsEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var req = new NewsEditRequest
        {
            Title = vm.Title,
            Summary = vm.Summary,
            BodyHtml = vm.BodyHtml,
            Category = vm.Category,
            Publish = vm.Publish,
            CreatedBy = User.Identity?.Name ?? "system",
        };

        if (vm.NewsId == null)
            await _api.CreateAsync(req);
        else
            await _api.UpdateAsync(vm.NewsId.Value, req);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
