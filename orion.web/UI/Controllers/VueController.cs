using Microsoft.AspNetCore.Mvc;

namespace orion.web.Controllers
{
    public class VueController : Controller
    {
        public IActionResult Index()
        {

            return View("App");
        }
    }
}
