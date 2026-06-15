using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Controllers;

public class AccountController : Controller
{
    private readonly IConfiguration _config;

    public AccountController(IConfiguration config) => _config = config;

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string userName, string password, string? returnUrl)
    {
        // 暫時用 appsettings AdminCredentials 驗證，待介接員工系統 API 後替換此段
        var cfgUser = _config["AdminCredentials:UserName"];
        var cfgHash = _config["AdminCredentials:PasswordHash"];
        var cfgRoles = _config["AdminCredentials:Roles"] ?? "Admin,Editor";

        bool valid = false;
        if (!string.IsNullOrEmpty(cfgUser) && !string.IsNullOrEmpty(cfgHash)
            && string.Equals(userName, cfgUser, StringComparison.OrdinalIgnoreCase))
        {
            var result = new PasswordHasher<object>().VerifyHashedPassword(null!, cfgHash, password);
            valid = result != PasswordVerificationResult.Failed;
        }

        if (!valid)
        {
            ModelState.AddModelError("", "帳號或密碼錯誤");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        var claims = new List<Claim> { new(ClaimTypes.Name, userName) };
        claims.AddRange(cfgRoles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denied() => View();
}
