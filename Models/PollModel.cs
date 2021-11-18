namespace EasyPoll.Models
{
    public class PollModel
    {
        public int Id { get; set; }
        public string PollName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime FinishAt { get; set; }
    }
}