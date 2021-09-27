﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using EasyPoll.Data;
using System.ComponentModel.DataAnnotations.Schema;

using MySql.Data.MySqlClient;
using MySql.Data.EntityFramework;

namespace EasyPoll.Models
{
    public class UserModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Key { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public string Token { get; set; }

        public UserModel() { }

        public UserModel(ViewModels.LoginViewModel viewModel)
        {
            UserModel user = GetUserModelByUsername(viewModel.Username);
            bool isPasswordCorrect = new PasswordHasher<UserModel>()
                .VerifyHashedPassword(user, user.Key, viewModel.Password) != PasswordVerificationResult.Failed;

            if (!isPasswordCorrect)
            {
                throw new System.Exception();
            }

            CopyUser(user);

            Token = GenerateToken();
            var usersDbContext = ServiceDBContext.GetDBContext();
            usersDbContext.Users.Update(this);
            usersDbContext.SaveChanges();
        }

        public static bool CheckUserToken(string token)
        {
            string username;
            System.DateTime tokenTime;
            try
            {
                var rawTokenData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(token)).Split(':');
                username = rawTokenData[1];
                var timestamp = rawTokenData[0];
                tokenTime = System.DateTime.FromBinary(long.Parse(timestamp));
            }
            catch
            {
                return false;
            }

            var user = GetUserModelByUsername(username);
            if (user == null && user.Token != token)
            { 
                return false; 
            }
            return tokenTime.AddHours(1) > System.DateTime.Now;
        }

        private static UserModel GetUserModelByUsername(string username)
        {
            var usersDbContext = ServiceDBContext.GetDBContext();
            return usersDbContext.Users.FirstOrDefaultAsync(user => user.Username == username).Result;
        }

        private string GenerateToken()
        {
            string rawToken = $"{System.DateTime.Now.ToBinary()}:{Username}";
            string token = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rawToken));
            return token;
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
