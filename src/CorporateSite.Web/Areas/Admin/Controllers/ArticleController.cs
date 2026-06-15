using CorporateSite.Web.Services;
using CorporateSite.Web.ViewModels.Article;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ArticleController : Controller
{
    private readonly IArticleService _api;
    public ArticleController(IArticleService api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? section = null, string? language = null)
    {
        var result = await _api.GetListAsync(page, pageSize: 20, section: section, language: language);
        ViewBag.Section  = section;
        ViewBag.Language = language;
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return View(new ArticleEditViewModel());

        var article = await _api.GetByIdAsync(id.Value);
        if (article == null) return NotFound();

        return View(new ArticleEditViewModel
        {
            ArticleId = article.ArticleId,
            Slug      = article.Slug,
            Title     = article.Title,
            Summary   = article.Summary,
            BodyHtml  = article.BodyHtml,
            Section   = article.Section,
            Language  = article.Language,
            SortOrder = article.SortOrder,
            Publish   = article.Status == 1,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ArticleEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var req = new ArticleEditRequest
        {
            Slug      = vm.Slug,
            Title     = vm.Title,
            Summary   = vm.Summary,
            BodyHtml  = vm.BodyHtml,
            Section   = vm.Section,
            Language  = vm.Language,
            SortOrder = vm.SortOrder,
            Publish   = vm.Publish,
            CreatedBy = User.Identity?.Name ?? "system",
        };

        (bool ok, string error) result;
        if (vm.ArticleId == null)
            result = await _api.CreateAsync(req);
        else
            result = await _api.UpdateAsync(vm.ArticleId.Value, req);

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
