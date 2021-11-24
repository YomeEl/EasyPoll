using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Identity;

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
        public IActionResult Register(ViewModels.RegisterViewModel model)
        {
            var newUser = new Models.UserModel()
            {
                Username = model.Username,
                Email = model.Email,
                DepartmentId = model.DepartmentId,
                RoleId = 1,
                Key = new PasswordHasher<Models.UserModel>().HashPassword(null, model.Password)
            };
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            dbcontext.Users.Add(newUser);
            dbcontext.SaveChanges();
            return RedirectToAction("Login", "Authentification");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Login", "Authentification");
        }

        [HttpPost]
        public IActionResult CheckRegistration()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var username = Request.Headers["Username"][0];
            var email = Request.Headers["Email"][0];
            var similarUsers = (from user in dbcontext.Users
                               where user.Username == username || user.Email == email
                               select user).ToArray();

            var usernameFound = false;
            var emailFound = false;
            foreach (var user in similarUsers)
            {
                usernameFound = usernameFound || user.Username == username;
                emailFound = emailFound || user.Email == email;
            }
            var strArr = new string[] { usernameFound.ToString(), emailFound.ToString() };
            var strVal = new Microsoft.Extensions.Primitives.StringValues(strArr);
            Response.Headers.Add("Result", strVal);

            return Ok(strVal.ToString().ToLower());
        }
    }
}