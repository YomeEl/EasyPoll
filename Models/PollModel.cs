using Microsoft.EntityFrameworkCore;

namespace EasyPoll.Models
{
    public class PollModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public System.DateTime Started { get; set; }
        public System.DateTime? Ended { get; set; }

        public System.Collections.Generic.List<AnswerModel> Answers { get; set; } 

        public void LoadAnswers()
        {
            var dbContext = Data.ServiceDBContext.GetDBContext();
            Answers = dbContext.Answers.FromSqlInterpolated($"SELECT * FROM dbo.Answers WHERE PollId = {Id}").ToListAsync().Result;
        }
    }
}