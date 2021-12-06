using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace EasyPoll.Controllers
{
    public class PollController : Controller
    {
        private readonly IWebHostEnvironment environment;

        public PollController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult ActivePoll()
        {
            var user = Models.UserModel.GetUserByToken(Request.Cookies["token"]);

            if (user == null || (user != null && !user.CheckToken()))
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

            var srcs = from path in Directory.GetFiles($"{environment.WebRootPath}\\img\\PollMedia\\{activePoll.Id}\\")
                       select $"/img/PollMedia/{activePoll.Id}/{Path.GetFileName(path)}";

            var data = new Dictionary<string, object>
            {
                ["pollId"] = activePoll.PollModel.Id,
                ["pollName"] = activePoll.PollModel.PollName,
                ["startAt"] = activePoll.PollModel.CreatedAt.ToString("u"),
                ["finishAt"] = activePoll.PollModel.FinishAt.ToString("u"),
                ["sendStart"] = false,
                ["sendFinish"] = false,
                ["questions"] = questions,
                ["options"] = activePoll.Options,
                ["media"] = null,
                ["answers"] = answers,
                ["userselection"] = userSelection,
                ["sources"] = srcs.ToArray(),
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

        public IActionResult UploadFile(IFormFile file, int pollId, int questionIndex)
        {
            var ext = Path.GetExtension(file.FileName);
            var path = $"{environment.WebRootPath}\\img\\PollMedia\\{pollId}\\{questionIndex}{ext}";
            var fs = new FileStream(path.ToString(), FileMode.CreateNew);
            file.CopyTo(fs);
            fs.Close();
            return Ok();
        }

        [HttpPost]
        public IActionResult AddNew(
            string oldName, string newName, 
            string startAtRaw, string finishAtRaw, 
            string sendStartRaw, string sendFinishRaw,
            string questionsRaw, string optionsRaw, string questionsChangedRaw)
        {
            var startAt = DateTime.Parse(startAtRaw);
            var finishAt = DateTime.Parse(finishAtRaw);
            var sendStart = bool.Parse(sendStartRaw);
            var sendFinish = bool.Parse(sendFinishRaw);
            var questions = (string[])JsonSerializer.Deserialize(questionsRaw, typeof(string[]));
            var options = (string[][])JsonSerializer.Deserialize(optionsRaw, typeof(string[][]));

            var questionsChanged = bool.Parse(questionsChangedRaw);

            var dbcontext = Data.ServiceDBContext.GetDBContext();

            var existingPoll = dbcontext.Polls.FirstOrDefaultAsync((poll) => poll.PollName == oldName).Result;
            int prevId = -1;
            if (existingPoll != null)
            {
                prevId = existingPoll.Id;
                if (!questionsChanged)
                {
                    existingPoll.PollName = newName;
                    existingPoll.CreatedAt = startAt;
                    existingPoll.FinishAt = finishAt;
                    dbcontext.Polls.Update(existingPoll);
                    dbcontext.SaveChanges();

                    return Ok();
                }
                dbcontext.Polls.Remove(existingPoll);
                dbcontext.SaveChanges();
            }
            
            var poll = new Models.PollModel()
            {
                PollName = newName,
                CreatedAt = startAt,
                FinishAt = finishAt
            };
            dbcontext.Polls.Add(poll);
            dbcontext.SaveChanges();
            poll = dbcontext.Polls.FirstAsync(p => p.PollName == newName).Result;
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

            var pathNew = $"{environment.WebRootPath}\\img\\PollMedia\\{poll.Id}\\";
            var pathOld = $"{environment.WebRootPath}\\img\\PollMedia\\{prevId}\\";
            if (prevId == -1)
            {
                Directory.CreateDirectory(pathNew);
            }
            else
            {
                Directory.Move(pathOld, pathNew);
            }

            return Ok(poll.Id);
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
