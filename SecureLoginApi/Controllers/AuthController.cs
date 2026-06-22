using Microsoft.AspNetCore.Mvc;
using SecureLoginApi.DTOs;
using SecureLoginApi.Services;

namespace SecureLoginApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // AuthService injiceras via konstruktorn (Dependency Injection)
        // Controller känner bara till AuthService - ingen logik här, bara ta emot och skicka vidare
        // Om byte vid implementation av AuthService behövs inte Controller-koden röras
        private readonly AuthService _authService;

        // ILogger är inbyggd i .NET - loggar misslyckade inloggningsförsök (OWASP A09)
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // AuthService hanterar BCrypt-hashning - lösenordet hashas innan det sparas
            var user = _authService.Register(request.Username, request.Password);
            return Ok(new { message = "Användare registrerad", username = user.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // AuthService verifierar lösenordet mot BCrypt-hashen i databasen
            var user = _authService.Login(request.Username, request.Password);

            if (user == null)
            {
                // Generiskt felmeddelande - avslöjar inte om det var username eller password som var fel (OWASP A07)
                // Loggar misslyckade försök för övervakning (OWASP A09)
                _logger.LogWarning("Misslyckat inloggningsförsök för användare: {Username}", request.Username);
                return Unauthorized(new { message = "Fel användarnamn eller lösenord" });
            }

            return Ok(new { message = "Inloggad", username = user.Username });
        }
    }
}
