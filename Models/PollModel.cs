using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPoll.Models
{
    public class PollModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string PollName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime FinishAt { get; set; }
        public bool SendStartRaw { get; set; }
        public bool SendFinishRaw { get; set; }
        public System.Collections.Generic.List<QuestionModel> Questions { get; set; }
    }
}