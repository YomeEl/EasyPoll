using Microsoft.AspNetCore.Mvc;

namespace EasyPoll.Controllers
{
    public class SettingsConroller : Controller
    {
        public IActionResult ControlPanel()
        {
            return View();
        }
    }
}
