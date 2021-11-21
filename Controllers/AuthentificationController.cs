using Microsoft.AspNetCore.Mvc;

namespace EasyPoll.Controllers
{
    public class AuthentificationController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ModelInvalid"] = false;
            return View();
        }

        [HttpPost]
        public IActionResult Login(ViewModels.LoginViewModel model)
        {
            if (model.IsValid())
            {
                Models.UserModel userModel;
                try 
                { 
                    userModel = new Models.UserModel(model); 
                }
                catch
                {
                    ViewData["ModelInvalid"] = true;
                    return View();
                }

                var opt = new Microsoft.AspNetCore.Http.CookieOptions();
                opt.Expires = new System.DateTimeOffset(System.DateTime.Now.Ticks, System.TimeSpan.FromHours(1));
                Response.Cookies.Append("token", userModel.Token, opt);
                return RedirectToAction("ActivePoll", "Poll");
            }
            else
            {
                ViewData["ModelInvalid"] = true;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["ModelInvalid"] = false;
            return View();
        }

        [HttpPost]
        public IActionResult Register(Models.RegisterModel model)
        {
            bool CheckResult = model.IsValid();
            if (CheckResult)
            {
                return RedirectToAction("Authentification", "Login");
            }
            else
            {
                ViewData["ModelInvalid"] = true;
                return View();
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Login", "Authentification");
        }
    }
}