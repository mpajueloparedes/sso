using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers.SSO
{
    public class TrainingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
