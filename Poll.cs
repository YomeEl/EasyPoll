using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace EasyPoll
{
    public class Poll
    {
        public int Id { get; }
        public Models.PollModel PollModel { get; }
        public string[] Questions { get; }
        public string[][] Options { get; }

        /// <summary>
        /// Indices [question][option][answer]
        /// </summary>
        public Models.AnswerModel[][][] Answers { get; }
        public Dictionary<int, int[]> UserAnswers { get; }
        public Dictionary<string, int[][]> AnswersByDepartmentName { get; }

        public Poll(int id)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            var u = dbcontext.Users.ToArray(); //Preload users to context

            var poll = dbcontext.Polls
                .Include(p => p.Questions).ThenInclude(q => q.Options)
                .Include(p => p.Questions).ThenInclude(q => q.Answers)
                .FirstOrDefault(p => p.Id == id);

            PollModel = poll;
            Id = id;
            Questions = poll.Questions.OrderBy(q => q.Order).Select(q => q.Question).ToArray();

            Options = new string[poll.Questions.Count][];
            var orderedQuestions = poll.Questions.OrderBy(q => q.Order).ToArray();
            for (int i = 0; i < Options.Length; i++)
            {
                Options[i] = orderedQuestions[i].Options
                    .OrderBy(o => o.Order)
                    .Select(o => o.Text)
                    .ToArray();
            }

            Answers = new Models.AnswerModel[Questions.Length][][];
            UserAnswers = new Dictionary<int, int[]>();
            for (int ans = 0; ans < Answers.Length; ans++)
            {
                var optionsCount = Options[ans].Length;
                Answers[ans] = new Models.AnswerModel[optionsCount][];
                for (int opt = 0; opt < optionsCount; opt++)
                {
                    Answers[ans][opt] = orderedQuestions[ans].Answers.Where(a => a.Answer == opt + 1).ToArray();
                }

                foreach (var answer in orderedQuestions[ans].Answers)
                {
                    if (!UserAnswers.ContainsKey(answer.UserId))
                    {
                        UserAnswers[answer.UserId] = new int[poll.Questions.Count];
                    }
                    UserAnswers[answer.UserId][ans] = answer.Answer;
                }
            }

            AnswersByDepartmentName = new Dictionary<string, int[][]>();
            var departments = dbcontext.Departments.ToArray();
            foreach (var dept in departments)
            {
                AnswersByDepartmentName[dept.Name] = GetAnswersAsCount(dept.Id);
            }
        }

        /// <summary>
        /// Indices [question][option]
        /// </summary>
        public int[][] GetAnswersAsCount(int departmentId = 0)
        {
            var result = new int[Questions.Length][];
            for (int i = 0; i < Questions.Length; i++)
            {
                result[i] = new int[Answers[i].Length];
                foreach (var opt in Answers[i])
                {
                    foreach (var ans in opt)
                    {
                        if (departmentId == 0 || ans.User.Department.Id == departmentId)
                        {
                            result[i][ans.Answer - 1]++;
                        }
                    }
                }
            }
            return result;
        }
    }
}