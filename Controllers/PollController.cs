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
            if (Request.Query.ContainsKey("answer"))
            {
                ViewData["Selected"] = int.Parse(Request.Query["answer"][0]);
            }
            else
            {
                ViewData["Selected"] = -1;
            }

            if (!HasValidToken())
            {
                return RedirectToAction("Login", "Authentification");
            }

            //TODO: Temporary. Active poll should be static somewhere.
            var activePoll = Data.ServiceDBContext.GetDBContext().Polls.FirstAsync().Result;
            ViewData["ActivePoll"] = activePoll;

            var questionList = (from question in Data.ServiceDBContext.GetDBContext().Questions
                                where question.PollId == activePoll.Id
                                select question).OrderBy(question => question.Id).ToArray();

            ViewData["Questions"] = questionList;

            return View();
        }

        [HttpPost]
        public IActionResult ActivePoll(int unused)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var activePoll = dbcontext.Polls.FirstAsync().Result;
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

        private bool HasValidToken()
        {
            var token = Request.Cookies["token"];
            return Models.UserModel.CheckUserToken(token);
        }
    }
}
