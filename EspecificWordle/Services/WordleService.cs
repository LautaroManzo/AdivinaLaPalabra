using Dapper;
using EspecificWordle.Interfaces;
using EspecificWordle.Models.ConfigApp;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Text;
using System.Data.SqlClient;
using EspecificWordle.DTOs;

namespace EspecificWordle.Services
{
    public class WordleService : IWordleService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ConfigApp _configApp;
        private readonly string _connectionString;

        public WordleService(ConfigApp configApp, IConfiguration configuration)
        {
            _configApp = configApp;
            _connectionString = configuration.GetConnectionString("HangfireConnection");
        }

        #region BD

        public async Task<AleatoriaDTO> GetAleatoriaAsync()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT * FROM GetAleatoria";
                var result = await dbConnection.QueryAsync<AleatoriaDTO>(sqlQuery);
                return result.First();
            }
        }

        #endregion BD

        #region Apis

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
                    return listWords.Any(w => RemoveAcentos(w.Word).Equals(word.ToLower()));
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar si la palabra existe.", ex);
            }
        }

        public string RemoveAcentos(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public class WordObject
        {
            public string Word { get; set; }
        }

        #endregion Apis

    }
}
