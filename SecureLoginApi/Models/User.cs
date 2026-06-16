namespace SecureLoginApi.Models
{
    public class User
    {
        // Unikt ID, primärnyckel i databasen
        public int Id { get; set; }

        // Användarnamnet
        public string Username { get; set; } = string.Empty;

        // Sparar aldrig klartext-lösenord, bara BCrypt-hashen
        public string PasswordHash { get; set; } = string.Empty;

        // Räknar misslyckade inloggningsförsök (för lockout)
        public int FailedLoginAttempts { get; set; } = 0;

        // Om inte null är kontot låst tills detta datum/tid
        public DateTime? LockoutUntil { get; set; }
    }
}
