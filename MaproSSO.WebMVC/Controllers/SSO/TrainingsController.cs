using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebMVC.Controllers.SSO
{
    public class TrainingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
