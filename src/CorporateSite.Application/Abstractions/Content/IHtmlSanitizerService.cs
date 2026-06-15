namespace CorporateSite.Application.Abstractions.Content;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
}
