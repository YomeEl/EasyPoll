using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace EasyPoll.Controllers
{
    public class PollController : Controller
    {
        [HttpGet]
        public IActionResult ActivePoll()
        {
            var user = Models.UserModel.GetUserByToken(Request.Cookies["token"]);
            int userId = user.Id;

            if (user == null || (user != null && !user.CheckToken()))
            {
                return RedirectToAction("Login", "Authentification");
            }

            var activePoll = Global.ActivePoll;
            bool answered = activePoll.UserAnswers.ContainsKey(userId);
            var answers = activePoll.GetAnswersAsCount();

            int totalCount = 0;
            foreach (var ans in answers[0])
            {
                totalCount += ans;
            }

            ViewData["ActivePoll"] = activePoll.PollModel;
            ViewData["Answered"] = answered;
            ViewData["TotalCount"] = totalCount;
            ViewData["ShowControlPanelButton"] = user.RoleId > 1;

            return View();
        }

        [HttpPost]
        public IActionResult ActivePoll(string answers)
        {
            var raw_answers = (string[])JsonSerializer.Deserialize(answers, typeof(string[]));
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            var userId = Models.UserModel.GetUserByToken(Request.Cookies["token"]).Id;
            for (int i = 0; i < raw_answers.Length; i++)
            {
                var ans = new Models.AnswerModel
                {
                    Answer = int.Parse(raw_answers[i]),
                    QuestionId = i + 1,
                    UserId = userId
                };
                dbcontext.Answers.Add(ans);
            };
            dbcontext.SaveChanges();

            Global.UpdateActivePoll();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetActivePollInfo()
        {
            var activePoll = Global.ActivePoll;

            var user = Models.UserModel.GetUserByToken(Request.Cookies["token"]);
            int userId = user.Id;

            var questions = activePoll.Questions;
            var answers = activePoll.GetAnswersAsCount();
            int[] userSelection = new int[activePoll.Questions.Length];
            bool answered = false;
            if (activePoll.UserAnswers.ContainsKey(userId))
            {
                userSelection = activePoll.UserAnswers[userId];
                answered = true;
            }

            var data = new Dictionary<string, object>();
            data["questions"] = questions;
            data["answers"] = answers;
            data["answered"] = answered;
            data["userselection"] = userSelection;

            return Ok(JsonSerializer.Serialize(data).ToLower());
        }

        public IActionResult PollControl()
        {
            return View();
        }

        public IActionResult AddNew()
        {
            return View();
        }

        public IActionResult ShowAll()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var pollsArray = dbcontext.Polls.OrderBy(poll => poll.CreatedAt).ToArray();
            ViewData["LastIsActive"] = pollsArray.Last().FinishAt > System.DateTime.Now;
            ViewData["PollsArray"] = pollsArray;
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
