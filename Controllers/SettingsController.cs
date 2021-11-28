using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

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

            return View();
        }

        public IActionResult GetUsers()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var users = (from user in dbcontext.Users
                         where user.RoleId != 1
                         select user).ToArray();
            var dict = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var user in users)
            {
                dict[user.Username] = user.RoleId;
            }
            return Ok(JsonSerializer.Serialize(dict));
        }

        public IActionResult CheckUsername(string username)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var res = dbcontext.Users.FirstOrDefault((user) => user.Username == username);
            return Ok((res != null).ToString().ToLower());
        }

        public IActionResult GetDepartments()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var depts = (from dept in dbcontext.Departments
                         orderby dept.Id
                         select System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(dept.Name)))
                         .ToArray();
            return Ok(JsonSerializer.Serialize(depts));
        }

        public IActionResult UpdateDepartments(string addRaw, string deleteRaw)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            var add = (string[])JsonSerializer.Deserialize(addRaw, typeof(string[]));
            var delete = (string[])JsonSerializer.Deserialize(deleteRaw, typeof(string[]));

            foreach (var dept in add)
            {
                var str = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dept));
                dbcontext.Departments.Add(new Models.DepartmentModel() { Name = str });
            }
            foreach (var dept in delete)
            {
                var str = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dept));
                var item = dbcontext.Departments.ToArray().First(i => i.Name == str);
                dbcontext.Departments.Remove(item);
            }
            dbcontext.SaveChanges();

            return Ok();
        }
    }
}