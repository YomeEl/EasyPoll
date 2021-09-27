using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;

using EasyPoll.ViewModels;

namespace EasyPoll.Models
{
    //TODO: Very bad implementation, review asap
    public class PollModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime? Ended { get; set; }
        public string Options { get; set; }

        private const string OPT_SEPARATOR = "~!";
        private int answersCount = -1;

        public List<AnswerViewModel> GetAnswers()
        {
            var options = Options.Split(OPT_SEPARATOR);
            var answers = LoadAnswers();
            var viewModels = new List<AnswerViewModel>();
            for (int i = 0; i < options.Length; i++)
            {
                viewModels.Add(new AnswerViewModel()
                {
                    Count = 0,
                    Text = options[i]
                });
            }
            foreach (var answer in answers)
            {
                viewModels[answer.Answer].Count++;
            }

            return viewModels;
        }

        public int GetTotalAnswersCount()
        {
            if (answersCount == -1)
            { 
                LoadAnswers(); 
            }
            return answersCount;
        }

        private List<AnswerModel> LoadAnswers()
        {
            var dbContext = Data.ServiceDBContext.GetDBContext();
            var res = dbContext.Answers.FromSqlInterpolated($"SELECT * FROM Answers WHERE PollId = {Id}").ToListAsync().Result;
            answersCount = res.Count;
            return res;
        }
    }
}