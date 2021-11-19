using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPoll
{
    public class Poll
    {
        public Models.PollModel PollModel { get; }
        public Models.QuestionModel[] Questions { get; }

        /// <summary>
        /// Indices [question][option][answer]
        /// </summary>
        public Models.AnswerModel[][][] Answers { get; set; }

        public Poll(int id)
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();

            PollModel = dbcontext.Polls.Find(id);
            Questions = (from question in dbcontext.Questions
                        where question.PollId == PollModel.Id
                        select question).OrderBy(q => q.Id).ToArray();

            Answers = new Models.AnswerModel[Questions.Length][][];
            for (int i = 0; i < Questions.Length; i++)
            {
                var currentQuestion = Questions[i];
                var optionsCount = currentQuestion.ExtractOptions().Length;

                var answers = (from answer in dbcontext.Answers
                           where answer.QuestionId == currentQuestion.Id
                           select answer).ToArray();
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
                result[i] = new int[Answers[0].GetLength(0)];
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