using Microsoft.AspNetCore.Mvc;

namespace CorporateSite.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}
