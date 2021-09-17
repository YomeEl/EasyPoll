using Microsoft.AspNetCore.Mvc;

namespace LightPoll.Controllers
{
    public class AuthentificationController : Controller
    {
        private readonly Data.UsersContext usersDbContext;

        public AuthentificationController(Data.UsersContext context)
        {
            usersDbContext = context;
        }

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
                    userModel = new Models.UserModel(model, usersDbContext); 
                }
                catch
                {
                    ViewData["ModelInvalid"] = true;
                    return View();
                }

                Response.Cookies.Append("token", "test");
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