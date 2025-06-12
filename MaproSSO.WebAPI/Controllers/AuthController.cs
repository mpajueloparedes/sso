using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
