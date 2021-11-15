using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EasyPoll.Models
{
    public class EditPollModel
    {
        
        [Required]
        public int QuestionPicture { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string QuestionText { get; set; }

        [Required]
        public List<string> Answers { get; set; }

        [Required]
        public List<int> AnswersPicture { get; set; }

        [Required]
        public bool Notification { get; set; }

        [Required]
        public int StartDate { get; set; }

        [Required]
        public int EndDate { get; set; }

        public bool IsValid()
        {
            return QuestionText != null && Answers != null;
        }
    }
}