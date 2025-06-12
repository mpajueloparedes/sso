using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers.SSO
{
    public class InspectionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
