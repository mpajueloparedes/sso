using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers
{
    public class TenantsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
