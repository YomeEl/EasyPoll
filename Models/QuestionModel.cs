using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EasyPoll.Models
{
    public class QuestionModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        public int PollId { get; set; }
        public PollModel Poll { get; set; }
        public int Order { get; set; }

        public System.Collections.Generic.List<OptionModel> Options { get; set; }
        public System.Collections.Generic.List<AnswerModel> Answers { get; set; }
    }
}
