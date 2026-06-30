using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
            try
            {
                // AuthService hanterar BCrypt-hashning - lösenordet hashas innan det sparas
                // AuthService kastar InvalidOperationException om användaren redan finns (VG-krav)
                var user = _authService.Register(request.Username, request.Password);
                return Ok(new { message = "Användare registrerad", username = user.Username });
            }
            catch (InvalidOperationException ex)
            {
                // Fångar dubblettfel och returnerar 409 Conflict istället för att krascha
                _logger.LogWarning("Registreringsförsök med redan existerande användare: {Username}", request.Username);
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]

        // Rate limiting (max 5 försök/minut) - skyddar mot brute force-attacker mot login (OWASP A07)
        [EnableRateLimiting("login")]
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
