using Microsoft.AspNetCore.Mvc;

namespace EasyPoll.Controllers
{
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult ControlPanel()
        {
            return View();
        }

        public IActionResult Service()
        {
            return View();
        }
    }
}