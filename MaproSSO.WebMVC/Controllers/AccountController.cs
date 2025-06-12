using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebMVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
