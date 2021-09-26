namespace EasyPoll.Models
{
    public class AnswerModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int PollId { get; set; }
        public int UserId { get; set; }
        public int Answer { get; set; }
    }
}
