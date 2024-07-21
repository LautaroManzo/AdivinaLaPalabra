using EspecificWordle.Interfaces;
using EspecificWordle.Models.ConfigApp;
using Newtonsoft.Json.Linq;
using Python.Runtime;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EspecificWordle.Services
{
    public class WordleService : IWordleService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ConfigApp _configApp;

        public WordleService(ConfigApp configApp)
        {
            _configApp = configApp;
        }

        #region Archivos

        public async Task<string> RandomWordleAsync()
        {
            try
            {
                var filePath = "C:\\Users\\USER\\Documents\\Projects\\EspecificWordle\\EspecificWordle\\FilesWords\\RandomWords.txt";
                var outputPath = "C:\\Users\\USER\\Documents\\Projects\\EspecificWordle\\EspecificWordle\\FilesWords\\OutRandomWords.txt";

                var words = (await File.ReadAllLinesAsync(filePath)).ToList();

                var randomIndex = new Random().Next(words.Count);
                var randomWord = words[randomIndex];

                // Se agrega la palabra random al archivo
                await File.AppendAllTextAsync(outputPath, randomWord + Environment.NewLine);

                // Se elimina la palabra random del archivo para que no se repita
                words.Remove(randomWord);

                await File.WriteAllLinesAsync(filePath, words);

                return randomWord;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la palabra aleatoria.", ex);
            }
        }

        #endregion Archivos

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

        #endregion Apis

        #region Python

        public async Task<string> GetDefinitionWord(string palabra)
        {
            var messageDefaults = new List<string>{ 
                "Versión electrónica 23.7 del «Diccionario de la lengua española», obra lexicográfica académica por excelencia.", 
                "Diccionario de la lengua española", "Definición RAE", "Entradas que contienen la forma" };

            try
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\USER\Documents\Projects\EspecificWordle\EspecificWordle\Python");
                dynamic functionsWords = Py.Import("FunctionsWords");

                // Definicion de la RAE
                dynamic def = functionsWords.wordDefinitionRae(palabra);

                if (!string.IsNullOrEmpty(def.ToString()) && !messageDefaults.Any(m => def.ToString().Contains(m)))
                {
                    var x = def.ToString();
                    return Regex.Replace(x, @"(?<!^)(\d+\. )", "\n$1");
                }
                else
                {
                    // Definicion de WORDNET
                    dynamic wordEn = (string)functionsWords.translateEn(palabra);
                    dynamic defEn = functionsWords.wordDefinition(wordEn);

                    if (!string.IsNullOrEmpty(defEn.ToString()))
                    {
                        dynamic result = functionsWords.translateEs(defEn.ToString());
                        return result.ToString().Replace(";", ",");
                    }
                    else
                        return string.Empty;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error RAE" + e.Message);
            }
        }

        public async Task<string> TranslateWord(string word)
        {
            try
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\USER\Documents\Projects\EspecificWordle\EspecificWordle\Python");

                dynamic functionsWords = Py.Import("FunctionsWords");
                dynamic wordEn = (string)functionsWords.translateEn(word);

                return wordEn;
            }
            catch (Exception e)
            {
                throw new Exception("Error al traducir de español a ingles " + e.Message);
            }
        }

        public async Task<List<string>> GetSynonymsWord(string word)
        {
            try
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\USER\Documents\Projects\EspecificWordle\EspecificWordle\Python");

                dynamic functionsWords = Py.Import("FunctionsWords");

                dynamic wordEn = (string)functionsWords.translateEn(word);

                dynamic result = functionsWords.wordSynonyms(wordEn);

                var pyList = new PyList(result);
                var listSynonyms = new List<string>();

                foreach (PyObject item in pyList)
                {
                    string palabra = item.As<string>().ToLower();
                    if (!palabra.Equals(word) && !listSynonyms.Contains(palabra) && !palabra.Contains("_"))
                        listSynonyms.Add(palabra);
                }

                return listSynonyms.Take(3).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Error al obtener sinonimos" + e.Message);
            }
        }

        public async Task<List<string>> GetAntonymsWord(string word)
        {
            try
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\USER\Documents\Projects\EspecificWordle\EspecificWordle\Python");

                dynamic functionsWords = Py.Import("FunctionsWords");

                dynamic wordEn = (string)functionsWords.translateEn(word);
                dynamic result = functionsWords.wordAntonyms(wordEn);

                var pyList = new PyList(result);
                var listAntonyms = new List<string>();

                foreach (PyObject item in pyList)
                {
                    string palabra = item.As<string>().ToLower();
                    if (!palabra.Equals(word) && !listAntonyms.Contains(palabra) && !palabra.Contains("_"))
                        listAntonyms.Add(palabra);
                }

                return listAntonyms.Take(3).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Error al obtener antonimos" + e.Message);
            }
        }

        public async Task<string> GetWordUseExample(string word)
        {
            try
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\USER\Documents\Projects\EspecificWordle\EspecificWordle\Python");

                dynamic functionsWords = Py.Import("FunctionsWords");
                dynamic wordEn = (string)functionsWords.translateEn(word);
                dynamic result = functionsWords.wordUseExample(wordEn);

                if (result == null)
                    return string.Empty;
                else
                    return (string)result;
            }
            catch (Exception e)
            {
                throw new Exception("Error al obtener usos de la palabra " + e.Message);
            }
        }

        #endregion Python

        public class WordObject
        {
            public string Word { get; set; }
        }

    }
}
