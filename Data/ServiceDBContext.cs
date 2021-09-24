using EasyPoll.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        public static ServiceDBContext GetDBContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ServiceDBContext>();
            optionsBuilder.UseSqlServer(AppSettings.Configuration.GetConnectionString("ServiceData"));
            return new ServiceDBContext(optionsBuilder.Options);
        }
    }
}