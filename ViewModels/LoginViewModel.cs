using System.ComponentModel.DataAnnotations;

namespace LightPoll.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsValid()
        {
            return Username != null && Password != null;
        }
    }
}