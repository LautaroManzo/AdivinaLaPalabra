namespace EspecificWordle.Interfaces
{
    public interface IWordleService
    {
        Task<string> RandomWordleAsync();

        Task<bool> WordCheckingAsync(string word, string language);

        Task<string> GetDefinitionWord(string word);

        Task<string> GetDefinitionRaeWord(string palabra);

        Task<string> TranslateWord(string word);

        Task<List<string>> GetSynonymsWord(string word);

        Task<List<string>> GetAntonymsWord(string word);

        Task<string> GetWordUseExample(string word);
    }
}
