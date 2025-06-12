using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers.SSO
{
    public class AnnouncementsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
