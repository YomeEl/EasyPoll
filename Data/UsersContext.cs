using LightPoll.Models;
using Microsoft.EntityFrameworkCore;

namespace LightPoll.Data
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