namespace SecureLoginApi.DTOs
{
    public class LoginRequest
    {
        // Användarnamnet som skickas in vid inloggning
        public string Username { get; set; } = string.Empty;

        // Lösenordet i klartext - jämförs mot den sparade hashen, sparas aldrig
        public string Password { get; set; } = string.Empty;
    }
}
