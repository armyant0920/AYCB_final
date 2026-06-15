using CorporateSite.Application.Abstractions.Content;
using Ganss.Xss;

namespace CorporateSite.Infrastructure.Content;

public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();

        _sanitizer.AllowedTags.UnionWith(new[]
        {
            "figure", "figcaption",
            "table", "thead", "tbody", "tr", "th", "td", "caption", "colgroup", "col"
        });

        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedAttributes.Add("style");
        _sanitizer.AllowedCssProperties.Add("text-align");
        _sanitizer.AllowedCssProperties.Add("margin");
        _sanitizer.AllowedCssProperties.Add("padding");

        // img.src: 只允許站內 /uploads/ 或 https
        _sanitizer.FilterUrl += (_, e) =>
        {
            if (e.OriginalUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;
            if (e.OriginalUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return;
            e.SanitizedUrl = null;
        };
    }

    public string Sanitize(string html) => _sanitizer.Sanitize(html);
}
