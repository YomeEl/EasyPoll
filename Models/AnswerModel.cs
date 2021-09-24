namespace EasyPoll.Models
{
    public class AnswerModel
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int UserId { get; set; }
        public int Answer { get; set; }
    }
}
