using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
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

            if (user == null || (user != null && !user.CheckToken()))
            {
                return RedirectToAction("Login", "Authentification");
            }

            int userId = user.Id;
            var activePoll = Global.ActivePoll;
            bool answered = activePoll.UserAnswers.ContainsKey(userId);
            var answers = activePoll.GetAnswersAsCount();

            int totalCount = 0;
            foreach (var ans in answers[0])
            {
                totalCount += ans;
            }

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

            var questions = (from question in dbcontext.Questions
                             where question.PollId == Global.ActivePoll.Id
                             select question.Id).ToArray();
            var userId = Models.UserModel.GetUserByToken(Request.Cookies["token"]).Id;
            for (int i = 0; i < raw_answers.Length; i++)
            {
                var ans = new Models.AnswerModel
                {
                    Answer = int.Parse(raw_answers[i]),
                    QuestionId = questions[i],
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

            if (activePoll.UserAnswers.ContainsKey(userId))
            {
                userSelection = activePoll.UserAnswers[userId];
            }

            var data = new Dictionary<string, object>
            {
                ["questions"] = questions,
                ["options"] = activePoll.Options,
                ["answers"] = answers,
                ["userselection"] = userSelection
            };

            return Ok(JsonSerializer.Serialize(data));
        }

        public IActionResult PollControl()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddNew()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddNew(
            string name, 
            string startAtRaw, string finishAtRaw, 
            string sendStartRaw, string sendFinishRaw,
            string questionsRaw, string optionsRaw)
        {
            var startAt = DateTime.Parse(startAtRaw);
            var finishAt = DateTime.Parse(finishAtRaw);
            var sendStart = bool.Parse(sendStartRaw);
            var sendFinish = bool.Parse(sendFinishRaw);
            var questions = (string[])JsonSerializer.Deserialize(questionsRaw, typeof(string[]));
            var options = (string[][])JsonSerializer.Deserialize(optionsRaw, typeof(string[][]));

            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var poll = new Models.PollModel()
            {
                PollName = name,
                CreatedAt = startAt,
                FinishAt = finishAt
            };
            dbcontext.Polls.Add(poll);
            dbcontext.SaveChanges();
            poll = dbcontext.Polls.FirstAsync(p => p.PollName == name).Result;
            for (int i = 0; i < questions.Length; i++)
            {
                var question = new Models.QuestionModel()
                {
                    PollId = poll.Id,
                    Question = questions[i]
                };
                dbcontext.Questions.Add(question);
                dbcontext.SaveChanges();
                question = dbcontext.Questions.FirstAsync(q => q.Question == questions[i]).Result;
                for (int j = 0; j < options[i].Length; j++)
                {
                    var option = new Models.OptionModel()
                    {
                        QuestionId = question.Id,
                        Text = options[i][j]
                    };
                    dbcontext.Options.Add(option);
                }
                dbcontext.SaveChanges();
            }

            return Ok();
        }

        public IActionResult ShowAll()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var pollsArray = dbcontext.Polls.OrderBy(poll => poll.CreatedAt).ToArray();
            ViewData["LastIsActive"] = pollsArray.Last().FinishAt > DateTime.Now;
            ViewData["PollsArray"] = pollsArray;
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
