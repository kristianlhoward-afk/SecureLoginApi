namespace SecureLoginApi.DTOs
{
    public class RegisterRequest
    {
        // Användarnamnet som skickas in vid registrering
        public string Username { get; set; } = string.Empty;

        // Lösenordet i klartext - hashas innan det sparas, skickas aldrig vidare som det är
        public string Password { get; set; } = string.Empty;
    }
}
