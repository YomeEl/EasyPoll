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
            "server=easypoll-db.mysql.database.azure.com;userid=app@easypoll-db;password=pwd;database=servicedata";

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