namespace CorporateSite.Web.Infrastructure;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var h = context.Response.Headers;
        h["X-Content-Type-Options"] = "nosniff";
        h["X-Frame-Options"] = "DENY";
        h["Referrer-Policy"] = "no-referrer";
        h["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

        // Admin 後台需要 CKEditor 動態 inline script；公開頁維持嚴格
        bool isAdmin = context.Request.Path.StartsWithSegments("/Admin");
        string scriptSrc = isAdmin ? "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " : "script-src 'self'; ";

        h["Content-Security-Policy"] =
            "default-src 'self'; " +
            "img-src 'self' data: blob:; " +
            "media-src 'self'; " +
            scriptSrc +
            "style-src 'self' 'unsafe-inline'; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'";

        await _next(context);
    }
}
