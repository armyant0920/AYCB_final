using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Editor")]
public class UploadController : Controller
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env) => _env = env;

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(5_242_880)]
    public async Task<IActionResult> Image(IFormFile upload)
    {
        if (upload == null || upload.Length == 0)
            return BadRequest(new { error = new { message = "未提供檔案" } });

        var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { error = new { message = $"不允許的副檔名：{ext}" } });

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_env.WebRootPath, "uploads", "images");
        Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, fileName);

        await using var fs = System.IO.File.Create(fullPath);
        await upload.CopyToAsync(fs);

        return Ok(new { url = $"/uploads/images/{fileName}" });
    }
}
