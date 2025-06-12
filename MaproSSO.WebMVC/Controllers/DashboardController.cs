using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebMVC.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
