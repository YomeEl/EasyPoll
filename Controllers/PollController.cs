using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net;

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

            var questions = Global.ActivePoll.PollModel.Questions
                .OrderBy(q => q.Order)
                .Select(q => q.Id)
                .ToArray();
            var userId = Models.UserModel.GetUserByToken(Request.Cookies["token"]).Id;

            var dbcontext = Data.ServiceDBContext.GetDBContext();

            var answersList = new List<Models.AnswerModel>();
            for (int i = 0; i < raw_answers.Length; i++)
            {
                answersList.Add(new Models.AnswerModel
                {
                    Answer = int.Parse(raw_answers[i]),
                    QuestionId = questions[i],
                    UserId = userId
                });
            };
            dbcontext.AddRange(answersList);
            dbcontext.SaveChanges();

            Global.UpdateActivePoll();
            return Ok();
        }

        [HttpGet]
        public IActionResult GetActivePollInfo()
        {
            return GetPollInfo();
        }

        [HttpGet]
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
            int pollId;
            Models.PollModel poll;
            if (existingPoll != null)
            {
                pollId = existingPoll.Id;

                existingPoll.PollName = newName;
                existingPoll.CreatedAt = startAt;
                existingPoll.FinishAt = finishAt;

                dbcontext.Polls.Update(existingPoll);

                if (!questionsChanged)
                {
                    Global.UpdateActivePoll();
                    return Ok();
                }

                dbcontext.Questions.RemoveRange(dbcontext.Questions.Where(q => q.PollId == pollId));
                dbcontext.SaveChanges();
            }
            else
            { 
                poll = new Models.PollModel()
                {
                    PollName = newName,
                    CreatedAt = startAt,
                    FinishAt = finishAt
                };
                dbcontext.Polls.Add(poll);
                dbcontext.SaveChanges();
                pollId = dbcontext.Polls.FirstAsync(p => p.PollName == newName).Result.Id;
            }

            var questionsList = new List<Models.QuestionModel>();
            for (int i = 0; i < questions.Length; i++)
            {
                var question = new Models.QuestionModel()
                {
                    PollId = pollId,
                    Question = questions[i],
                    Order = i,
                    Options = new List<Models.OptionModel>()
                };

                for (int j = 0; j < options[i].Length; j++)
                {
                    var option = new Models.OptionModel()
                    {
                        Text = options[i][j],
                        Order = j
                    };
                    question.Options.Add(option);
                }

                questionsList.Add(question);
            }
            dbcontext.Questions.AddRange(questionsList);
            dbcontext.SaveChanges();

            Global.UpdateActivePoll();
            return Ok(pollId);
        }

        [HttpGet]
        public IActionResult ShowAll()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var pollsArray = dbcontext.Polls.OrderBy(poll => poll.CreatedAt).ToArray();
            ViewData["LastIsActive"] = pollsArray.Last().FinishAt > DateTime.Now;
            ViewData["PollsArray"] = pollsArray;
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewData["pollId"] = id;
            return View();
        }

        public IActionResult UploadFile(IFormFile file, int pollId, int questionIndex)
        {
            var filename = $"{pollId}_{questionIndex}";
            var ext = Path.GetExtension(file.FileName);
            var url = GeneratePreSignedURL(1, filename + ext, HttpVerb.PUT);
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[8000];
                var fileStream = file.OpenReadStream();
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dataStream.Write(buffer, 0, bytesRead);
                }
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;

            var dbcontext = Data.ServiceDBContext.GetDBContext();
            dbcontext.MediaExtMapping.Add(new Models.MediaExtMappingModel
            {
                Filename = filename,
                Extension = ext
            });
            dbcontext.SaveChanges();

            return Ok(response);
        }

        public IActionResult DeleteFiles(string questionsRaw, string pollId)
        {
            var questions = (int[])JsonSerializer.Deserialize(questionsRaw, typeof(int[]));
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            var mem = dbcontext.MediaExtMapping.ToArray();
            HttpWebResponse response = null;
            foreach (var q in questions)
            {
                var map = mem.FirstOrDefault(m => m.Filename == $"{pollId}_{q}");
                if (map != null && map.Extension != "")
                {
                    var url = GeneratePreSignedURL(1, $"{map.Filename}{map.Extension}", HttpVerb.DELETE);
                    HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
                    httpRequest.Method = "DELETE";
                    response = httpRequest.GetResponse() as HttpWebResponse;
                    dbcontext.MediaExtMapping.Remove(map);
                }
            }
            dbcontext.SaveChanges();
            return Ok(response);
        }

        //If id is not provided, returns info for Global.ActivePoll
        public IActionResult GetPollInfo(int id = 0)
        {
            var poll = id == 0 || id == Global.ActivePoll.Id ? Global.ActivePoll : new Poll(id);

            var user = Models.UserModel.GetUserByToken(Request.Cookies["token"]);
            int userId = user.Id;

            var questions = poll.Questions;
            var answers = poll.GetAnswersAsCount();
            int[] userSelection = new int[poll.Questions.Length];

            if (poll.UserAnswers.ContainsKey(userId))
            {
                userSelection = poll.UserAnswers[userId];
            }

            var srcs = new List<string>();

            var dbcontext = Data.ServiceDBContext.GetDBContext();
            for (int i = 0; i < questions.Length; i++)
            {
                var filename = $"{poll.Id}_{i}";
                var mem = dbcontext.MediaExtMapping.FirstOrDefault((m) => m.Filename == filename);
                srcs.Add(mem != null ? GeneratePreSignedURL(0.1d, $"{mem.Filename}{mem.Extension}", HttpVerb.GET) : "");
            }

            var data = new Dictionary<string, object>
            {
                ["pollId"] = poll.PollModel.Id,
                ["pollName"] = poll.PollModel.PollName,
                ["startAt"] = poll.PollModel.CreatedAt.ToString("u"),
                ["finishAt"] = poll.PollModel.FinishAt.ToString("u"),
                ["sendStart"] = false,
                ["sendFinish"] = false,
                ["questions"] = questions,
                ["options"] = poll.Options,
                ["media"] = null,
                ["answers"] = answers,
                ["answersByDepartment"] = poll.AnswersByDepartmentName,
                ["userselection"] = userSelection,
                ["sources"] = srcs.ToArray(),
            };

            return Ok(JsonSerializer.Serialize(data));
        }

        private static string GeneratePreSignedURL(double duration, string filename, HttpVerb verb)
        {
            const string bucketName = "elasticbeanstalk-eu-central-1-871792599540";
            IAmazonS3 s3Client = new AmazonS3Client(RegionEndpoint.EUCentral1);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = filename,
                Verb = verb,
                Expires = DateTime.UtcNow.AddHours(duration)
            };

            string url = s3Client.GetPreSignedURL(request);
            return url;
        }
    }
}
