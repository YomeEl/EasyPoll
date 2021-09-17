using Microsoft.AspNetCore.Mvc;

namespace EasyPoll.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
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
            return token != null;
        }
    }
}
