using SecureLoginApi.Models;

namespace SecureLoginApi.Repositories
{
    // IF-kontrakt. Alla klasser som implementerar detta MÅSTE ha dessa metoder.
    // Tack vare detta InMemory bytas ut mot SQL senare utan att röra Controller eller Service.
    public interface IUserRepository
    {
        // Hämtar en användare baserat på username. Returnerar null om den inte finns.
        User? GetByUsername(string username);

        // Sparar en ny användare
        void Add(User user);

        // Kollar om ett username redan är taget
        bool Exists(string username);
    }
}
