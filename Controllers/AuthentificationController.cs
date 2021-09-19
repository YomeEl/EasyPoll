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

                Response.Cookies.Append("token", userModel.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["ModelInvalid"] = true;
                return View();
            }
        }
    }
}