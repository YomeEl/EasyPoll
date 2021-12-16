using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using EasyPoll.Models;

namespace EasyPoll.Controllers
{
    public class PollApiController : BaseController
    {
        [HttpPost]
        public IActionResult AnswerPoll(string answers)
        {
            var raw_answers = (string[])JsonSerializer.Deserialize(answers, typeof(string[]));

            var questions = Global.ActivePoll.PollModel.Questions
                .OrderBy(q => q.Order)
                .Select(q => q.Id)
                .ToArray();
            var userId = user.Id;

            var answersList = new List<AnswerModel>();
            for (int i = 0; i < raw_answers.Length; i++)
            {
                answersList.Add(new AnswerModel
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

        [HttpPost]
        public IActionResult AddNewPoll(
            string oldName, string newName,
            string startAtRaw, string finishAtRaw,
            string sendStartRaw, string sendFinishRaw,
            string questionsRaw, string optionsRaw, string questionsChangedRaw)
        {
            if (user == null || user.RoleId != 3)
            {
                return BadRequest();
            }

            var startAt = DateTime.Parse(startAtRaw);
            var finishAt = DateTime.Parse(finishAtRaw);
            var sendStart = bool.Parse(sendStartRaw);
            var sendFinish = bool.Parse(sendFinishRaw);
            var questions = (string[])JsonSerializer.Deserialize(questionsRaw, typeof(string[]));
            var options = (string[][])JsonSerializer.Deserialize(optionsRaw, typeof(string[][]));

            var questionsChanged = bool.Parse(questionsChangedRaw);

            var existingPoll = dbcontext.Polls.Where(poll => poll.PollName == oldName).FirstOrDefault();
            int pollId;
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
                var poll = new PollModel()
                {
                    PollName = newName,
                    CreatedAt = startAt,
                    FinishAt = finishAt
                };
                dbcontext.Polls.Add(poll);
                dbcontext.SaveChanges();
                pollId = dbcontext.Polls.FirstAsync(p => p.PollName == newName).Result.Id;
            }

            UpdateQuestions(pollId, questions, options);

            Global.UpdateActivePoll();
            return Ok(pollId);
        }

        public IActionResult UploadMedia(IFormFile file, int pollId, int questionIndex, int optionIndex = -1)
        {
            if (user == null || user.RoleId != 3)
            {
                return BadRequest();
            }

            var filename = $"{pollId}_{questionIndex}";
            if (optionIndex != -1)
            {
                filename += $"_{optionIndex}";
            }
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

            dbcontext.MediaExtMapping.Add(new MediaExtMappingModel
            {
                Filename = filename,
                Extension = ext
            });
            dbcontext.SaveChanges();

            return Ok(response);
        }

        public IActionResult DeleteMedia(string questionsRaw, string optionsRaw, string pollId)
        {
            if (user == null || user.RoleId != 3)
            {
                return BadRequest();
            }

            var questions = (int[])JsonSerializer.Deserialize(questionsRaw, typeof(int[]));
            var options = (int[][])JsonSerializer.Deserialize(optionsRaw, typeof(int[][]));
            var mem = dbcontext.MediaExtMapping.ToArray();

            var urls = new List<string>();
            foreach (var q in questions)
            {
                var map = mem.FirstOrDefault(m => m.Filename == $"{pollId}_{q}");
                if (map != null && map.Extension != "")
                {
                    urls.Add(GeneratePreSignedURL(1, $"{map.Filename}{map.Extension}", HttpVerb.DELETE));
                }
                dbcontext.MediaExtMapping.Remove(map);
            }

            for (int q = 0; q < options.Length; q++)
            {
                foreach (var opt in options[q])
                {
                    var map = mem.FirstOrDefault(m => m.Filename == $"{pollId}_{q}_{opt}");
                    if (map != null && map.Extension != "")
                    {
                        urls.Add(GeneratePreSignedURL(1, $"{map.Filename}{map.Extension}", HttpVerb.DELETE));
                    }
                    dbcontext.MediaExtMapping.Remove(map);
                }
            }

            dbcontext.SaveChanges();

            HttpWebResponse response = null;
            foreach (var url in urls)
            {
                HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
                httpRequest.Method = "DELETE";
                response = httpRequest.GetResponse() as HttpWebResponse;
            }

            return Ok(response);
        }

        //If id is not provided, returns info for Global.ActivePoll
        public IActionResult GetPollInfo(int id = 0)
        {
            if (user == null || (user.RoleId == 1 && id != 0))
            {
                return BadRequest();
            }

            var poll = id == 0 || id == Global.ActivePoll.Id ? Global.ActivePoll : new Poll(id);

            int userId = user.Id;

            var questions = poll.Questions;
            var answers = poll.GetAnswersAsCount();
            int[] userSelection = new int[poll.Questions.Length];

            if (poll.UserAnswers.ContainsKey(userId))
            {
                userSelection = poll.UserAnswers[userId];
            }

            var srcs = new List<string>();
            var optionSources = new List<string>[questions.Length];
            for (int i = 0; i < questions.Length; i++)
            {
                var filename = $"{poll.Id}_{i}";
                var mem = dbcontext.MediaExtMapping.FirstOrDefault(m => m.Filename == filename);
                var link = "";
                if (mem != null)
                {
                    link = GeneratePreSignedURL(0.1d, $"{mem.Filename}{mem.Extension}", HttpVerb.GET);
                }
                srcs.Add(link);
                optionSources[i] = new List<string>();
                for (int j = 0; j < poll.Options[i].Length; j++)
                {
                    filename = $"{poll.Id}_{i}_{j}";
                    mem = dbcontext.MediaExtMapping.FirstOrDefault(m => m.Filename == filename);
                    link = "";
                    if (mem != null)
                    {
                        link = GeneratePreSignedURL(0.1d, $"{mem.Filename}{mem.Extension}", HttpVerb.GET);
                    }
                    optionSources[i].Add(link);
                }
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
                ["answers"] = answers,
                ["answersByDepartment"] = poll.AnswersByDepartmentName,
                ["userselection"] = userSelection,
                ["sources"] = srcs.ToArray(),
                ["optionSources"] = optionSources.Select(s => s.ToArray()).ToArray()
            };

            return Ok(JsonSerializer.Serialize(data));
        }

        public IActionResult GetTable(int id)
        {
            if (user == null || user.RoleId == 1)
            {
                return BadRequest();
            }

            var questions = dbcontext.Questions
                .Where(q => q.PollId == id)
                .Include(q => q.Answers).ThenInclude(a => a.User).ThenInclude(u => u.Department)
                .Include(q => q.Options)
                .OrderBy(q => q.Order)
                .ToList();

            var tableBuilder = new TableBuider(questions);
            var workbook = tableBuilder.Build();

            var s = new MemoryStream();
            workbook.Write(s, true);
            s.Seek(0, SeekOrigin.Begin);

            return File(s, "application/vnd.ms-excel", "answers.xlsx");
        }

        private void UpdateQuestions(int pollId, string[] questions, string[][] options)
        {
            var questionsList = new List<QuestionModel>();
            for (int i = 0; i < questions.Length; i++)
            {
                var question = new QuestionModel()
                {
                    PollId = pollId,
                    Question = questions[i],
                    Order = i,
                    Options = new List<OptionModel>()
                };

                for (int j = 0; j < options[i].Length; j++)
                {
                    var option = new OptionModel()
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
