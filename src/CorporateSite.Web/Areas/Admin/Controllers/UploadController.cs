using CorporateSite.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Editor")]
public class UploadController : Controller
{
    private readonly IFileStorageService _storage;
    public UploadController(IFileStorageService storage) => _storage = storage;

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(5_242_880)]
    public async Task<IActionResult> Image(IFormFile upload)
    {
        if (upload == null || upload.Length == 0)
            return BadRequest(new { error = new { message = "未提供檔案" } });

        try
        {
            var saved = await _storage.SaveAsync(
                upload.OpenReadStream(), upload.FileName, upload.ContentType, FileKind.Image);
            return Ok(new { url = saved.PublicUrl });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = new { message = ex.Message } });
        }
    }
}
