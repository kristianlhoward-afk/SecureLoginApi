using Microsoft.EntityFrameworkCore;
using SecureLoginApi.Models;

namespace SecureLoginApi.Data
{
    // DbContext = EF Core:s bro mellan C#-koden och databasen
    // DbSet<User> blir tabellen "Users" i databasen
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
