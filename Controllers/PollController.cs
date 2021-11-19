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

            if (user == null || (user != null && !user.CheckToken()))
            {
                return RedirectToAction("Login", "Authentification");
            }

            //TODO: Temporary. Active poll should be static somewhere.
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var activePoll = dbcontext.Polls.FirstAsync().Result;
            ViewData["ActivePoll"] = activePoll;

            var questions = (from question in dbcontext.Questions
                                where question.PollId == activePoll.Id
                                select question).OrderBy(question => question.Id).ToArray();

            int userId = user.Id;
            var userSelection = new int[questions.Length];
            var answers = new int[questions.Length][];
            for (int i = 0; i < questions.Length; i++)
            {
                var qAns = (from answer in dbcontext.Answers
                            where answer.QuestionId == questions[i].Id
                            select answer).ToArray();
                answers[i] = new int[questions[i].Options.Split("~!").Length];
                foreach (var ans in qAns)
                {
                    answers[i][ans.Answer - 1]++;
                    if (ans.UserId == userId)
                    {
                        userSelection[i] = ans.Answer;
                    }
                }
            }

            int totalCount = 0;
            foreach (var ans in answers[0])
            {
                totalCount += ans;
            }

            var answered = (from answer in dbcontext.Answers
                           where answer.UserId == userId
                           select answer).Count() != 0;

            if (answered)
            {
                ViewData["Selected"] = 1;
            }

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
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
