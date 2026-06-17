using Microsoft.AspNetCore.Mvc;
using SecureLoginApi.Models;
using SecureLoginApi.DTOs;
using SecureLoginApi.Repositories;

namespace SecureLoginApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Repository injiceras via konstruktorn (Dependency Injection)
        // Controller känner bara till interfacet IUserRepository - inte vilken implementation som körs
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // OBS: ingen BCrypt eller dubblettkontroll än
            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.Password // TILLFÄLLIGT - sparas i klartext just nu!
            };

            _userRepository.Add(user);

            return Ok(new { message = "Användare registrerad", username = user.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userRepository.GetByUsername(request.Username);

            // OBS: ingen BCrypt-jämförelse än - bara enkel strängjämförelse just nu
            if (user == null || user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Fel användarnamn eller lösenord" });
            }

            return Ok(new { message = "Inloggad", username = user.Username });
        }
    }
}
