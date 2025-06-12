using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebAPI.Controllers
{
    public class SubscriptionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
