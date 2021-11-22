using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;

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
            var questions = activePoll.Questions;
            bool answered = false;
            var answers = activePoll.GetAnswersAsCount();
            int[] userSelection = new int[activePoll.Questions.Length];
            if (activePoll.UserAnswers.ContainsKey(userId))
            {
                userSelection = activePoll.UserAnswers[userId];
                answered = true;
            }

            int totalCount = 0;
            foreach (var ans in answers[0])
            {
                totalCount += ans;
            }

            ViewData["ActivePoll"] = activePoll.PollModel;
            ViewData["Answered"] = answered;
            ViewData["Questions"] = questions;
            ViewData["Answers"] = answers;
            ViewData["UserSelection"] = userSelection;
            ViewData["TotalCount"] = totalCount;
            ViewData["ShowControlPanelButton"] = user.RoleId > 1;

            return View();
        }

        [HttpPost]
        public IActionResult ActivePoll(int unused)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var raw_answers = Request.Form["Answers"][0].Split(',');
            for (int i = 0; i < raw_answers.Length; i++)
            {
                var ans = new Models.AnswerModel
                {
                    Answer = int.Parse(raw_answers[i]),
                    QuestionId = i + 1,
                    UserId = 1
                };
                dbcontext.Answers.Add(ans);
            };
            dbcontext.SaveChanges();

            Global.UpdateActivePoll();

            return Ok();
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
