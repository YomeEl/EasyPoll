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
    }
}