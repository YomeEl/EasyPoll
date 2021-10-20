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

        public IActionResult PollControl()
        {
            return RedirectToAction("PollControl", "Poll");
        }

        public IActionResult AddNewPoll()
        {
            return RedirectToAction("AddNew", "Poll");
        }

        public IActionResult ShowAll()
        {
            return RedirectToAction("ShowAll", "Poll");
        }

        public IActionResult GoBack()
        {
            return RedirectToAction("ActivePoll", "Poll");
        }
    }
}