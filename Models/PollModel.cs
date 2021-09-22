namespace EasyPoll.Models
{
    public class PollModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public System.DateTime Started { get; set; }
        public System.DateTime? Ended { get; set; }
    }
}