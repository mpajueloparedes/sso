using Microsoft.AspNetCore.Mvc;

namespace MaproSSO.WebMVC.Controllers
{
    public class SubscriptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
