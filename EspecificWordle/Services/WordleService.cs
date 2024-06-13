using EspecificWordle.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace EspecificWordle.Services
{
    public class WordleService : IWordleService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> RandomWordleAsync(string lengthWordle, string language)
        {
            try
            {
                var url = $"https://api.datamuse.com/words?sp={lengthWordle}&v={language}";     // https://www.datamuse.com/api/

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var listWords = JArray.Parse(json).ToObject<List<WordObject>>();
                
                // Se obtiene una palabra random de la lista que retorna la API.
                var word = listWords != null && listWords.Count > 0 ? listWords[new Random().Next(0, listWords.Count)].Word : string.Empty;

                return word;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la palabra aleatoria.", ex);
            }
        }

        public async Task<bool> WordCheckingAsync(string word, string language)
        {
            try
            {
                var url = $"https://api.datamuse.com/words?sp={word}&v={language}";
                var exist = false;

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var listWords = JArray.Parse(json).ToObject<List<WordObject>>();

                if (listWords != null && listWords.Count > 0)
                    exist = listWords.First().Word.Equals(word.ToLower());

                return exist;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar si la palabra existe.", ex);
            }
        }

        public class WordObject
        {
            public string Word { get; set; }
        }

    }
}
