using SecureLoginApi.Models;
using System.Collections.Concurrent;

namespace SecureLoginApi.Repositories
{
    // Minnesbaserad lagring med Dictionary.
    // ConcurrentDictionary istället för vanlig Dictionary - trådsäker om flera requests kommer samtidigt.
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users = new();
        private int _nextId = 1;

        public User? GetByUsername(string username)
        {
            _users.TryGetValue(username, out var user);
            return user;
        }

        public void Add(User user)
        {
            user.Id = _nextId++;
            _users[user.Username] = user;
        }

        public bool Exists(string username)
        {
            return _users.ContainsKey(username);
        }
    }
}
