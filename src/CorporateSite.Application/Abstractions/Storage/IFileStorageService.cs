namespace CorporateSite.Application.Abstractions.Storage;

public enum FileKind { Image, Video }

public class StoredFile
{
    public string PublicUrl { get; init; } = "";
    public string RelativePath { get; init; } = "";
}

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(Stream content, string originalName, string contentType, FileKind kind);
}
