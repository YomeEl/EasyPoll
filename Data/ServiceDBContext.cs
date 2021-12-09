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
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<OptionModel> Options { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<MediaExtMappingModel> MediaExtMapping { get; set; }

        private const string connectionString =
            "server=easypoll-db.cad8zcrqvclk.eu-central-1.rds.amazonaws.com;" +
            "userid=app;password=pwd;" +
            "database=easypoll;" +
            "SSL Mode=None;" +
            "CharSet=utf8;";

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