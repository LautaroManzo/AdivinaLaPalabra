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
                var url = $"https://clientes.api.greenborn.com.ar/public-random-word?l={lengthWordle}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var wordArray = JsonConvert.DeserializeObject<string[]>(json);
                var word = string.Empty;

                if (wordArray != null && wordArray.Length > 0)
                {
                    word = wordArray[0];

                    // Si la palabra obtenida tiene acento se realiza otra llamada recursiva.
                    if (ContainsAccentedCharacters(word))
                        return await RandomWordleAsync(lengthWordle, language);
                }

                return word;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error al realizar la solicitud HTTP.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error al deserializar la respuesta JSON.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la palabra aleatoria.", ex);
            }
        }

        public bool ContainsAccentedCharacters(string input)
        {
            var acentos = new char[] { 'á', 'é', 'í', 'ó', 'ú' };

            foreach (var acento in acentos)
            {
                if (input.Contains(acento))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> WordCheckingAsync(string word, string language)
        {
            try
            {
                var url = $"https://api.datamuse.com/words?sp={word}&v={language}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var listWords = JArray.Parse(json).ToObject<List<WordObject>>();

                if (listWords != null && listWords.Count > 0)
                    return listWords.Any(w => w.Word.Equals(word.ToLower()));
                else
                    return false;
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
