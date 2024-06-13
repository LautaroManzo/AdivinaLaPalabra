namespace EspecificWordle.Interfaces
{
    public interface IWordleService
    {
        Task<string> RandomWordleAsync(string lengthWordle, string language);

        Task<bool> WordCheckingAsync(string word, string language);
    }
}
