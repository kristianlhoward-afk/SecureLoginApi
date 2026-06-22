using SecureLoginApi.Models;
using SecureLoginApi.Repositories;

namespace SecureLoginApi.Services
{
    // All säkerhetslogik samlad här - Controller anropar bara dessa metoder
    // och behöver inte veta HUR lösenord hashas eller verifieras
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Hashar lösenordet med BCrypt innan användaren sparas  //(Hash = destruktiv omvandling. Går aldrig att få tillbaka originalvärdet.)
        // BCrypt genererar automatiskt ett unikt salt per lösenord - skyddar mot rainbow table-attacker. (Salt = ett unikt slumpmässigt värde
        //      läggs till varje lösenord innan det hashas
        public User Register(string username, string password)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            _userRepository.Add(user);
            return user;
        }

        // Verifierar inloggning genom att jämföra lösenordet mot den sparade hashen
        // Returnerar null vid fel - Controller avgör sedan vilket (generiskt) felmeddelande som visas
        public User? Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);

            if (user == null)
            {
                return null;
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordCorrect)
            {
                return null;
            }

            return user;
        }
    }
}
