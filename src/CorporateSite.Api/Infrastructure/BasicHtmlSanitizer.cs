using System.Text.RegularExpressions;

namespace CorporateSite.Api.Infrastructure;

/// <summary>
/// Lightweight HTML sanitizer using regex allowlist approach.
/// Replace with HtmlSanitizer (Ganss.Xss) when the NuGet package is available.
/// </summary>
public static class BasicHtmlSanitizer
{
    // Tags that are allowed to pass through
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

        // 1. Remove <script> blocks entirely (including content)
        html = Regex.Replace(html, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase);

        // 2. Remove <style> blocks
        html = Regex.Replace(html, @"<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase);

        // 3. Remove <!-- comments -->
        html = Regex.Replace(html, @"<!--[\s\S]*?-->", "");

        // 4. Strip on* event attributes (onclick, onload, onerror, etc.)
        html = Regex.Replace(html, @"\s+on\w+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]*)", "",
            RegexOptions.IgnoreCase);

        // 5. Strip javascript: and data: in href/src
        html = Regex.Replace(html, @"(href|src)\s*=\s*[""']\s*(?:javascript|data|vbscript):[^""']*[""']", "",
            RegexOptions.IgnoreCase);

        // 6. Remove tags that are not in the allowlist
        html = Regex.Replace(html, @"<(/?)(\w+)([^>]*)>", m =>
        {
            string tag = m.Groups[2].Value;
            if (AllowedTags.Contains(tag))
                return m.Value;
            return "";
        }, RegexOptions.IgnoreCase);

        return html;
    }
}
