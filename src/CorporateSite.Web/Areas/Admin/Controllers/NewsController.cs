using CorporateSite.Web.Services;
using CorporateSite.Web.ViewModels.News;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class NewsController : Controller
{
    private readonly NewsApiClient _api;
    public NewsController(NewsApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? category = null, string? language = null)
    {
        var result = await _api.GetListAsync(page, pageSize: 20, category: category, language: language);
        ViewBag.Category = category;
        ViewBag.Language = language;
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
            NewsId   = news.NewsId,
            Title    = news.Title,
            Summary  = news.Summary,
            BodyHtml = news.BodyHtml,
            Category = news.Category,
            Language = news.Language,
            Publish  = news.Status == 1,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NewsEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var req = new NewsEditRequest
        {
            Title    = vm.Title,
            Summary  = vm.Summary,
            BodyHtml = vm.BodyHtml,
            Category = vm.Category,
            Language = vm.Language,
            Publish  = vm.Publish,
            CreatedBy = User.Identity?.Name ?? "system",
        };

        (bool ok, string error) result;
        if (vm.NewsId == null)
            result = await _api.CreateAsync(req);
        else
            result = await _api.UpdateAsync(vm.NewsId.Value, req);

        if (!result.ok)
        {
            ModelState.AddModelError("", $"儲存失敗：{result.error}");
            return View(vm);
        }

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
