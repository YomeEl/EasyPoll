using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyPoll.Controllers
{
    public class PollController : Controller
    {
        [HttpGet]
        public IActionResult ActivePoll()
        {
            if (Request.Query.ContainsKey("answer"))
            {
                ViewData["Selected"] = int.Parse(Request.Query["answer"][0]);
            }
            else
            {
                ViewData["Selected"] = -1;
            }

            if (HasValidToken())
            {
                //TODO: Temporary. Active poll should be static somewhere.
                ViewData["ActivePoll"] = Data.ServiceDBContext.GetDBContext().Polls.FirstAsync().Result;
                
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Authentification");
            }
        }

        public IActionResult AddNew()
        {
            return View();
        }

        public IActionResult ShowAll()
        {
            return View();
        }

        private bool HasValidToken()
        {
            var token = Request.Cookies["token"];
            return Models.UserModel.CheckUserToken(token);
        }
    }
}
