using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
