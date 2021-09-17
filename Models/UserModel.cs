using Microsoft.EntityFrameworkCore;

namespace LightPoll.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public UserModel() { }
        public UserModel(ViewModels.LoginViewModel viewModel, Data.UsersContext usersDbContext)
        {
            UserModel user = usersDbContext.Users.FromSqlInterpolated(
                $"SELECT * FROM dbo.Users WHERE Username={viewModel.Username}").ToListAsync().Result[0];

            if (user.Password != viewModel.Password)
            {
                throw new System.Exception();
            }

            Id = user.Id;
            Username = user.Username;
            Password = user.Password;
        }
    }
}
