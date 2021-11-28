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
            if (!(Request.Cookies.ContainsKey("token") && Global.CheckSU(Request.Cookies["token"])))
            {
                return RedirectToAction("Login", "Authentification");
            }
            return View();
        }
    }
}