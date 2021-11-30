using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

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
            var dict = new Dictionary<string, int>();
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
                         select dept.Name).ToArray();
            return Ok(JsonSerializer.Serialize(depts));
        }

        public IActionResult GetUsersWithoutDepartment()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var users = (from user in dbcontext.Users
                         where user.DepartmentId == null
                         select user.Username).ToArray();
            return Ok(JsonSerializer.Serialize(users));
        }

        public IActionResult UpdateUserDepartments(string usersRaw, string departmentsRaw)
        {
            var users = (string[])JsonSerializer.Deserialize(usersRaw, typeof(string[]));
            var deptNames = (string[])JsonSerializer.Deserialize(departmentsRaw, typeof(string[]));
            var depts = ConvertDepartmentsToIds(deptNames);
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            for (int i = 0; i < users.Length; i++)
            {
                var user = (from u in dbcontext.Users
                            where u.Username == users[i]
                            select u).First();
                user.DepartmentId = depts[i];
                dbcontext.Users.Update(user);
            }
            dbcontext.SaveChanges();

            return Ok();
        }

        public IActionResult UpdateDepartments(string addRaw, string deleteRaw)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            var add = (string[])JsonSerializer.Deserialize(addRaw, typeof(string[]));
            var delete = (string[])JsonSerializer.Deserialize(deleteRaw, typeof(string[]));

            foreach (var dept in add)
            {
                dbcontext.Departments.Add(new Models.DepartmentModel() { Name = dept });
            }
            foreach (var dept in delete)
            {
                var item = dbcontext.Departments.ToArray().First(i => i.Name == dept);
                dbcontext.Departments.Remove(item);
            }
            dbcontext.SaveChanges();

            return Ok();
        }

        private int[] ConvertDepartmentsToIds(string[] departments)
        {
            var ids = new int[departments.Length];
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            for (int i = 0; i < departments.Length; i++)
            {
                ids[i] = (from dept in dbcontext.Departments
                          where dept.Name == departments[i]
                          select dept.Id).First();
            }
            return ids;
        }
    }
}