using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.IO;
using System.Linq;

namespace EasyPoll.Controllers
{
    public class PollController : BaseController
    {
        [HttpGet]
        public IActionResult ActivePoll()
        {
            if (user == null)
            {
                return RedirectToAction("Login", "Authentification");
            }

            int userId = user.Id;

            ViewData["ShowControlPanelButton"] = user.RoleId > 1;
            var activePoll = Global.ActivePoll;
            if (activePoll == null)
            {
                ViewData["NoActivePoll"] = true;
                return View();
            }

            bool answered = activePoll.UserAnswers.ContainsKey(userId);
            var answers = activePoll.GetAnswersAsCount();

            int totalCount = 0;
            foreach (var ans in answers[0])
            {
                totalCount += ans;
            }

            ViewData["Answered"] = answered;
            ViewData["TotalCount"] = totalCount;
            ViewData["NoActivePoll"] = false;

            return View();
        }

        [HttpGet]
        public IActionResult PollControl()
        {
            if (user == null || user.RoleId != 3)
            {
                return Ok("Доступ запрещён!");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AddNew()
        {
            if (user == null || user.RoleId != 3)
            {
                return Ok("Доступ запрещён!");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ShowAll()
        {
            if (user == null || user.RoleId == 1)
            {
                return Ok("Доступ запрещён!");
            }

            var pollsArray = dbcontext.Polls.OrderBy(poll => poll.CreatedAt).ToArray();
            ViewData["LastIsActive"] = pollsArray.Last().FinishAt > DateTime.Now;
            ViewData["PollsArray"] = pollsArray;
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if (user == null || user.RoleId == 1)
            {
                return Ok("Доступ запрещён!");
            }

            ViewData["pollId"] = id;
            return View();
        }
    }
}
