using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var depts = (from dep in dbcontext.Departments
                         where dep.Id > 0
                         select dep.Name).ToArray();
            var deptsString = "";
            foreach (var d in depts)
            {
                deptsString += d + '\n';
            }
            
            ViewData["Departments"] = deptsString.Trim('\n');

            return View();
        }
    }
}