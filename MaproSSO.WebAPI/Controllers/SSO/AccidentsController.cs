using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers.SSO
{
    public class AccidentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
