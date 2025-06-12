using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers
{
    public class BaseApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
