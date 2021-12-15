using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPoll.Models
{
    public class OptionModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public QuestionModel Question { get; set; }
        public int? MediaId { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
    }
}
