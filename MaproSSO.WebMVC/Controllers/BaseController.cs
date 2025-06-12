using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebMVC.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
