using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPoll.Models
{
    public class AnswerModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public QuestionModel Question { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public int Answer { get; set; }
    }
}
