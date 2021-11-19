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
        public string Options { get; set; }
        public int PollId { get; set; }

        const string SEPARATOR = "~!";

        public string[] ExtractOptions()
        {
            return Options.Split(SEPARATOR);
        }
    }
}
