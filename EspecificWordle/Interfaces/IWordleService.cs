using DataBase.Models;
using EspecificWordle.DTOs;

namespace EspecificWordle.Interfaces
{
    public interface IWordleService
    {
        Task UpdateRandomWordDaily();

        Task<WordDTO> GetModeWordDetailsAsync(int modoId);

        Task<bool> WordCheckingAsync(string word, string language);

        Task<Modo> GetModoByDescripcion(string modoDescripcion);

        Task<List<Modo>> GetModosAsync();
    }
}
