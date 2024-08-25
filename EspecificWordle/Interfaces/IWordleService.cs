using EspecificWordle.DTOs;

namespace EspecificWordle.Interfaces
{
    public interface IWordleService
    {
        Task<AleatoriaDTO> GetAleatoriaAsync();

        Task<bool> WordCheckingAsync(string word, string language);
    }
}
