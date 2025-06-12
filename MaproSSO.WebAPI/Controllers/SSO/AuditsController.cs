using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers.SSO
{
    public class AuditsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
