using Microsoft.EntityFrameworkCore;
using SecureLoginApi.Data;
using SecureLoginApi.Models;

namespace SecureLoginApi.Repositories
{
    // SQL-baserad implementation av IUserRepository - sparar i Azure SQL via EF Core
    // Eftersom Controller och Service bara känner till interfacet IUserRepository
    // kunde vi byta ut InMemoryUserRepository mot denna utan att ändra någon annan kod
    public class SqlUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public SqlUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public bool Exists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
    }
}
