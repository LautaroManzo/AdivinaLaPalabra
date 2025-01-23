using EspecificWordle.Interfaces;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Text;
using EspecificWordle.DTOs;
using Microsoft.EntityFrameworkCore;
using DataBase.Models;
using System;

namespace EspecificWordle.Services
{
    public class WordleService : IWordleService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly WordGameContext _context;

        public WordleService(IConfiguration configuration, WordGameContext context)
        {
            _context = context;
        }

        #region BD

        public async Task UpdateRandomWordDaily()
        {
            try
            {
                var modos = _context.Modo.ToList();

                foreach (var modo in modos)
                {
                    var word = _context.Word.Where(x => x.Usada == false && x.ModoId == modo.Id)
                        .OrderBy(x => Guid.NewGuid())   // Selecciona una Word random
                        .FirstOrDefault();

                    if (word != null)
                    {
                        word.Usada = true;

                        var wordMode = _context.WordMode.Where(x => x.ModoId == modo.Id).FirstOrDefault();

                        if (wordMode == null)
                        {
                            _context.WordMode.Add(new WordMode
                            {
                                ModoId = modo.Id,
                                WordId = word.Id,
                                Fecha = DateTime.Now
                            });
                        }
                        else
                        {
                            wordMode.WordId = word.Id;
                            wordMode.Fecha = DateTime.Now;
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WordDTO> GetModeWordDetailsAsync(int modoId)
        {
            var wordMode = _context.WordMode.Where(x => x.ModoId == modoId).FirstOrDefault();
            
            var dto = _context.Word.Where(wm => wm.ModoId == modoId && wm.Id == wordMode.WordId)
                .Select(wm => new WordDTO
                {
                    ModeId = wm.ModoId.Value,
                    Palabra = wm.Descripcion,
                    Definicion = wm.Definicion.Descripcion,
                    PalabraEn = wm.PalabraEn.Descripcion,
                    Pista = wm.Pista.Descripcion,
                    EjemploUso = wm.Uso.Descripcion,
                    Sinonimos = string.Join(", ", wm.Sinonimo.Select(s => s.Descripcion.ToLower())),
                    Antonimos = string.Join(", ", wm.Antonimo.Select(a => a.Descripcion.ToLower()))
                }).FirstOrDefault();

            return dto;
        }

        public async Task<Modo> GetModoByDescripcion(string modoDescripcion)
        {
            return await _context.Modo.FirstOrDefaultAsync(m => m.Descripcion == modoDescripcion);
        }

        private void AddWord(string word, int modoId, string descripcion, string usoEjemplo, string pista, string wordEn, List<string> sinonimos, List<string> antonimos)
        {
            try
            {
                var newWord = new Word
                {
                    Descripcion = word,
                    ModoId = modoId,
                    Usada = false,
                    Definicion = new Definicion { Descripcion = descripcion },
                    PalabraEn = new PalabraEn { Descripcion = wordEn },
                    Pista = new Pista { Descripcion = pista },
                    Uso = new Uso { Descripcion = usoEjemplo },
                    Sinonimo = sinonimos.Select(s => new Sinonimo { Descripcion = s }).ToList(),
                    Antonimo = antonimos.Select(a => new Antonimo { Descripcion = a }).ToList()
                };

                _context.Word.Add(newWord);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
