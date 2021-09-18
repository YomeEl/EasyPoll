using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LightPoll.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Key { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }

        public UserModel() { }

        public UserModel(ViewModels.LoginViewModel viewModel, Data.UsersContext usersDbContext)
        {
            UserModel user = usersDbContext.Users.FromSqlInterpolated(
                $"SELECT * FROM dbo.Users WHERE Username={viewModel.Username}").ToListAsync().Result[0];

            var key = new PasswordHasher<UserModel>().HashPassword(user, viewModel.Password);

            bool isPasswordCorrect = new PasswordHasher<UserModel>()
                .VerifyHashedPassword(user, user.Key, viewModel.Password) != PasswordVerificationResult.Failed;

            if (!isPasswordCorrect)
            {
                throw new System.Exception();
            }

            CopyUser(user);
        }

        private void CopyUser(UserModel other)
        {
            Id = other.Id;
            Username = other.Username;
            Key = other.Key;
            RoleId = other.RoleId;
            DepartmentId = other.DepartmentId;
        }
    }
}
