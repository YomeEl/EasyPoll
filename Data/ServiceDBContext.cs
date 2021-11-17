using EasyPoll.Models;

using Microsoft.EntityFrameworkCore;

namespace EasyPoll.Data
{
    public class ServiceDBContext : DbContext
    {
        public ServiceDBContext(DbContextOptions<ServiceDBContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<PollModel> Polls { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }

        private const string connectionString =
            "server=sql11.freemysqlhosting.net;userid=sql11451768;password=5ah1S9Zu6Z;database=sql11451768;SSL Mode=None;";

        public static ServiceDBContext GetDBContext()
        {
            return new ServiceDBContext();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(connectionString);
        }

        private ServiceDBContext() : base()
        {

        }
    }
}