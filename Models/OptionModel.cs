using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EasyPoll.Models
{
    public class OptionModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int? MediaId { get; set; }
        public string Text { get; set; }
        public int Order { get; set; }
    }
}
