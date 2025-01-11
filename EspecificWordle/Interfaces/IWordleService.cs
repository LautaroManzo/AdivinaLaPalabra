using EspecificWordle.DTOs;

namespace EspecificWordle.Interfaces
{
    public interface IWordleService
    {
        Task<bool> UpdateRandomWordDaily();

        Task<WordModeDTO> GetModeWordDetailsAsync(int modoId);

        Task<bool> WordCheckingAsync(string word, string language);

        Task<ModoDTO> GetModoByDescripcion(string modoDescripcion);
    }
}
