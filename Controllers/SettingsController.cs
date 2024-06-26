﻿using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using EasyPoll.Models;

namespace EasyPoll.Controllers
{
    public class SettingsController : BaseController
    {
        [HttpGet]
        public IActionResult ControlPanel()
        {
            if (user == null || user.RoleId == 1)
            {
                return Ok("Доступ запрещён");
            }

            ViewData["NoActivePoll"] = Global.ActivePoll == null;
            return View();
        }

        [HttpGet]
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
            if (!(Request.Cookies.ContainsKey("token") && Global.CheckSU(Request.Cookies["token"])))
            {
                return Ok("Доступ запрещён");
            }

            var users = dbcontext.Users.Where(user => user.RoleId != 1).ToArray();
            var dict = new Dictionary<string, int>();
            foreach (var user in users)
            {
                dict[user.Username] = user.RoleId;
            }
            return Ok(JsonSerializer.Serialize(dict));
        }

        public IActionResult CheckUsername(string username)
        {
            if (!(Request.Cookies.ContainsKey("token") && Global.CheckSU(Request.Cookies["token"])))
            {
                return Ok("Доступ запрещён");
            }

            var res = dbcontext.Users.FirstOrDefault((user) => user.Username == username);
            return Ok((res != null).ToString().ToLower());
        }

        public IActionResult GetDepartments()
        {
            if (!(Request.Cookies.ContainsKey("token") && Global.CheckSU(Request.Cookies["token"])))
            {
                return Ok("Доступ запрещён");
            }

            var depts = dbcontext.Departments.OrderBy(dept => dept.Id).ToArray();
            return Ok(JsonSerializer.Serialize(depts));
        }

        public IActionResult UpdateDepartments(string addRaw, string deleteRaw)
        {
            if (!(Request.Cookies.ContainsKey("token") && Global.CheckSU(Request.Cookies["token"])))
            {
                return Ok("Доступ запрещён");
            }

            var add = (string[])JsonSerializer.Deserialize(addRaw, typeof(string[]));
            var delete = (string[])JsonSerializer.Deserialize(deleteRaw, typeof(string[]));

            foreach (var dept in add)
            {
                dbcontext.Departments.Add(new DepartmentModel() { Name = dept });
            }
            foreach (var dept in delete)
            {
                var item = dbcontext.Departments.ToArray().First(i => i.Name == dept);
                dbcontext.Departments.Remove(item);
            }
            dbcontext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateRoles(string itemsRaw)
        {
            if (!(Request.Cookies.ContainsKey("token") && Global.CheckSU(Request.Cookies["token"])))
            {
                return Ok("Доступ запрещён");
            }

            var items = (UserModel[])JsonSerializer.Deserialize(itemsRaw, typeof(UserModel[]));
            foreach (var item in items)
            {
                var user = dbcontext.Users.FirstOrDefault(u => u.Username == item.Username);
                if (user != null)
                {
                    user.RoleId = item.RoleId;
                    dbcontext.Update(user);
                }
            }
            dbcontext.SaveChanges();

            return Ok();
        }
    }
}