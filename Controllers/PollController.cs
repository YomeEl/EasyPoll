using Microsoft.AspNetCore.Mvc;

namespace EasyPoll.Controllers
{
    public class PollController : Controller
    {
        [HttpGet]
        public IActionResult ActivePoll()
        {
            if (HasValidToken())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Authentification");
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Login", "Authentification");
        }

        private bool HasValidToken()
        {
            var token = Request.Cookies["token"];
            return Models.UserModel.CheckUserToken(token);
        }
    }
}
