using CorporateSite.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Hosting;

namespace CorporateSite.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private static readonly string[] AllowedVideoExtensions = [".mp4", ".webm"];

    private static readonly Dictionary<string, byte[]> MagicNumbers = new()
    {
        { ".jpg",  [0xFF, 0xD8, 0xFF] },
        { ".jpeg", [0xFF, 0xD8, 0xFF] },
        { ".png",  [0x89, 0x50, 0x4E, 0x47] },
        { ".gif",  [0x47, 0x49, 0x46] },
        { ".webp", [0x52, 0x49, 0x46, 0x46] },
        { ".webm", [0x1A, 0x45, 0xDF, 0xA3] },
    };

    private readonly IWebHostEnvironment _env;
    public LocalFileStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<StoredFile> SaveAsync(Stream content, string originalName, string contentType, FileKind kind)
    {
        var ext = Path.GetExtension(originalName).ToLowerInvariant();
        var allowed = kind == FileKind.Image ? AllowedImageExtensions : AllowedVideoExtensions;

        if (!allowed.Contains(ext))
            throw new InvalidOperationException($"不允許的副檔名：{ext}");

        // Magic number 驗證（讀檔頭後重建可讀 stream）
        var header = new byte[8];
        int read = await content.ReadAsync(header.AsMemory(0, 8));
        ValidateMagic(ext, header.AsSpan(0, read));

        Stream seekable;
        if (content.CanSeek)
        {
            content.Seek(0, SeekOrigin.Begin);
            seekable = content;
        }
        else
        {
            var ms = new MemoryStream();
            ms.Write(header, 0, read);
            await content.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            seekable = ms;
        }

        var subfolder = kind == FileKind.Image ? "images" : "videos";
        var datePart = DateTime.UtcNow.ToString("yyyy/MM");
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine("uploads", subfolder, datePart, fileName);
        var fullPath = Path.Combine(_env.WebRootPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var fs = File.Create(fullPath);
        await seekable.CopyToAsync(fs);

        return new StoredFile
        {
            RelativePath = relativePath,
            PublicUrl = "/" + relativePath.Replace(Path.DirectorySeparatorChar, '/')
        };
    }

    private static void ValidateMagic(string ext, ReadOnlySpan<byte> header)
    {
        if (!MagicNumbers.TryGetValue(ext, out var magic)) return;
        if (header.Length < magic.Length || !header[..magic.Length].SequenceEqual(magic))
            throw new InvalidOperationException("檔案內容與副檔名不符");
    }
}
