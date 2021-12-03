using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Poll(int id)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            PollModel = dbcontext.Polls.Find(id);
            Id = id;
            var questionModels = (from question in dbcontext.Questions
                        where question.PollId == PollModel.Id
                        select question).OrderBy(q => q.Id).ToArray();
            Questions = questionModels.Select(q => q.Question).ToArray();
            Options = new string[questionModels.Length][];
            for (int i = 0; i < questionModels.Length; i++)
            {
                var questionId = questionModels[i].Id;
                Options[i] = (from opt in dbcontext.Options
                              where opt.QuestionId == questionId
                              select opt.Text).ToArray();
            }

            Answers = new Models.AnswerModel[questionModels.Length][][];
            UserAnswers = new Dictionary<int, int[]>();
            for (int i = 0; i < questionModels.Length; i++)
            {
                var currentQuestion = questionModels[i];
                var optionsCount = Options[i].Length;

                var answers = (from answer in dbcontext.Answers
                           where answer.QuestionId == currentQuestion.Id
                           select answer).ToArray();
                foreach (var ans in answers)
                {
                    if (!UserAnswers.ContainsKey(ans.UserId))
                    {
                        UserAnswers[ans.UserId] = new int[questionModels.Length];
                    }
                    UserAnswers[ans.UserId][i] = ans.Answer;
                }
                Answers[i] = new Models.AnswerModel[optionsCount][];
                for (int opt = 0; opt < optionsCount; opt++)
                {
                    Answers[i][opt] = (from answer in answers
                                       where answer.Answer == (opt + 1)
                                       select answer).ToArray();
                }
            }
        }

        public int[][] GetAnswersAsCount()
        {
            var result = new int[Questions.Length][];
            for (int i = 0; i < Questions.Length; i++)
            {
                result[i] = new int[Answers[i].GetLength(0)];
                foreach (var opt in Answers[i])
                {
                    foreach (var ans in opt)
                    {
                        result[i][ans.Answer - 1]++;
                    }
                }
            }
            return result;
        }
    }
}