using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebMVC.Controllers.SSO
{
    public class AnnouncementsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
