using EasyPoll.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyPoll.Data
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
    }
}