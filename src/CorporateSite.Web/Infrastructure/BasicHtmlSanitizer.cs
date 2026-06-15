using System.Text.RegularExpressions;

namespace CorporateSite.Web.Infrastructure;

/// <summary>
/// Lightweight HTML sanitizer. Replace with HtmlSanitizer (Ganss.Xss) when available.
/// </summary>
public static class BasicHtmlSanitizer
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "p","br","b","i","strong","em","u","s","ul","ol","li",
        "h1","h2","h3","h4","h5","h6","blockquote","pre","code",
        "table","thead","tbody","tr","th","td","caption",
        "a","img","figure","figcaption","hr","span","div"
    };

    public static string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return "";

        html = Regex.Replace(html, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<!--[\s\S]*?-->", "");
        html = Regex.Replace(html, @"\s+on\w+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]*)", "",
            RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"(href|src)\s*=\s*[""']\s*(?:javascript|data|vbscript):[^""']*[""']", "",
            RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<(/?)(\w+)([^>]*)>", m =>
        {
            string tag = m.Groups[2].Value;
            return AllowedTags.Contains(tag) ? m.Value : "";
        }, RegexOptions.IgnoreCase);

        return html;
    }
}
