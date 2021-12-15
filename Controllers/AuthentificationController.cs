using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using System;
using System.Text;
using System.Linq;

using EasyPoll.Models;
using EasyPoll.ViewModels;

namespace EasyPoll.Controllers
{
    public class AuthentificationController : BaseController
    {
        private readonly CookieOptions cookieOptions = new() { Expires = new DateTimeOffset(DateTime.Now.AddHours(1)) };

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ModelInvalid"] = false;
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            var suToken = Global.CheckSUAndGenerateToken(viewModel.Username, viewModel.Password);
            if (suToken.Length > 0)
            {
                Response.Cookies.Append("token", suToken, cookieOptions);
                return RedirectToAction("Service", "Settings");
            }

            UserModel userModel = GetUserModel(viewModel);
            if (userModel == null)
            {
                ViewData["ModelInvalid"] = true;
                return View();
            }
            
            Response.Cookies.Append("token", userModel.Token, cookieOptions);
            return RedirectToAction("ActivePoll", "Poll");
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["ModelInvalid"] = false;
            ViewData["Departments"] = dbcontext.Departments.OrderBy((d) => d.Id).ToArray();
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            var newUser = new UserModel()
            {
                Username = model.Username,
                Email = model.Email,
                DepartmentId = model.DepartmentId,
                RoleId = 1,
                Key = new PasswordHasher<UserModel>().HashPassword(null, model.Password)
            };
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
        public IActionResult CheckRegistration(string username, string email)
        {
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

            return Ok($"{usernameFound},{emailFound}");
        }
    
        public static UserModel GetUserByToken(string token)
        {
            if (token == null) return null;
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            return dbcontext.Users.Where(user => user.Token == token).FirstOrDefault();
        }

        //Also updates token in db
        private UserModel GetUserModel(LoginViewModel viewModel)
        {
            var user = dbcontext.Users.Where(user => user.Username == viewModel.Username).FirstOrDefault();

            bool isPasswordCorrect = new PasswordHasher<UserModel>()
                .VerifyHashedPassword(user, user.Key, viewModel.Password) != PasswordVerificationResult.Failed;


            if (user == null || !isPasswordCorrect)
            {
                return null;
            }

            user.Token = GenerateToken(viewModel.Username);
            dbcontext.Users.Update(user);
            dbcontext.SaveChanges();

            return user;
        }

        private static string GenerateToken(string username)
        {
            string rawToken = $"{DateTime.Now.ToBinary()}:{username}";
            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawToken));
            return token;
        }
    }
}