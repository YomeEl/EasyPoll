using System.ComponentModel.DataAnnotations;

namespace EasyPoll.Models
{
    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string RepeatedPassword { get; set; }

        [Required]
        public int Department { get; set; }

        public bool IsValid()
        {
            return EmailAdress != null && Username != null && Password != null && Password == RepeatedPassword;
        }



    }
}