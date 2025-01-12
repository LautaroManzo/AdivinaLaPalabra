using EspecificWordle.Interfaces;
using EspecificWordle.Models;
using EspecificWordle.Models.Wordle;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EspecificWordle.Controllers
{
    public class WordleController : Controller
    {
        private readonly ILogger<WordleController> _logger;
        private readonly IWordleService _IWordleService;

        public WordleController(ILogger<WordleController> logger, IWordleService iWordleService)
        {
            _logger = logger;
            _IWordleService = iWordleService;
        }

        [Route("{modoDescripcion}")]
        public async Task<IActionResult> Index(string modoDescripcion)
        {
            var modoId = (await _IWordleService.GetModoByDescripcion(modoDescripcion)).Id;
            var word = CleanWord((await _IWordleService.GetModeWordDetailsAsync(modoId)).Palabra);

            var viewModel = new WordleViewModel
            {
                Length = word.Length,
                ModoId = modoId,
                ModoDescripcion = modoDescripcion,
                GameFinish = false,
                Juego = new Dictionary<string, List<Session>>()
            };

            #region Obtención de cookie

            var listSession = GetGameFromCookie(viewModel.ModoId);

            if (listSession.Count > 0)
                viewModel.Juego.Add(viewModel.ModoId.ToString(), listSession);

            viewModel.JuegoDictionaryJson = System.Text.Json.JsonSerializer.Serialize(viewModel.Juego);

            if (viewModel.Juego != null && viewModel.Juego.ContainsKey(modoId.ToString()))
            {
                viewModel.Juego = viewModel.Juego;
                viewModel.Intentos = viewModel.Juego[modoId.ToString()].Count;

                // Buscar otra forma de hacer esto
                if (viewModel.Juego[modoId.ToString()].Last().WordInsert.ToUpper() == word.ToUpper())
                {
                    viewModel.Resultado = 1;
                    viewModel.GameFinish = true;
                }
                else if (viewModel.Juego[modoId.ToString()].Count == 5 && viewModel.Juego[modoId.ToString()].Last().WordInsert.ToUpper() != word.ToUpper())
                {
                    viewModel.Resultado = 0;
                    viewModel.GameFinish = true;
                }
            }
            else
            {
                viewModel.Intentos = 0;
            }

            #endregion

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(WordleViewModel wordleViewModel)
        {
            var exist = await _IWordleService.WordCheckingAsync(wordleViewModel.PalabraIngresada, "es");
            var wordSecret = CleanWord((await _IWordleService.GetModeWordDetailsAsync(wordleViewModel.ModoId)).Palabra.ToUpper());

            if (wordleViewModel.JuegoDictionaryJson != null)
                wordleViewModel.Juego = JsonConvert.DeserializeObject<Dictionary<string, List<Session>>>(wordleViewModel.JuegoDictionaryJson);

            if (!wordleViewModel.Juego.ContainsKey(wordleViewModel.ModoId.ToString()))
                wordleViewModel.Juego.Add(wordleViewModel.ModoId.ToString(), new List<Session>());

            if (exist)
            {
                // Letras donde la posicion es correcta/incorrecta                    
                List<Letter> letters = new List<Letter>();

                for (int i = 0; i < wordSecret.Length; i++)
                {
                    if (wordSecret[i] == wordleViewModel.PalabraIngresada[i])
                    {
                        letters.Add(new Letter() {
                            Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                            Color = "Verde",
                            Acerted = true
                        });
                    }
                    else
                    {
                        if (wordSecret.Contains(wordleViewModel.PalabraIngresada[i]))
                        {
                            bool isLetterRepeatedInYellowOrGray = letters.Any(letter =>
                                letter.Letra == wordleViewModel.PalabraIngresada[i].ToString() &&
                                letter.Color != "Verde");

                            bool isLetterRepeatedInGreen = letters.Any(letter =>
                                letter.Letra == wordleViewModel.PalabraIngresada[i].ToString() &&
                                letter.Color == "Verde");

                            bool isLetterRepeatedInWordSecret = wordSecret.GroupBy(character => character).Any(group => group.Count() > 1);

                            if ((!isLetterRepeatedInWordSecret && isLetterRepeatedInGreen) || isLetterRepeatedInYellowOrGray)
                            {
                                // 1er condición: Si la palabra oculta no repite alguna letra && Si el usuario ingresó una palabra que sí repite alguna letra y esta letra es correcta (Verde)
                                // 2da condición: Si el usuario ingresó una palabra que sí repite alguna letra y ésta letra ya se marcó (Amarilla o Gris)
                                letters.Add(new Letter()
                                {
                                    Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                    Color = "Gris",
                                    Acerted = false
                                });
                            }
                            else
                            {
                                // Contiene la letra de la posición [i]
                                letters.Add(new Letter()
                                {
                                    Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                    Color = "Amarillo",
                                    Acerted = false
                                });
                            }
                        }
                        else
                        {
                            // No contiene la letra
                            letters.Add(new Letter()
                            {
                                Letra = wordleViewModel.PalabraIngresada[i].ToString(),
                                Color = "Gris",
                                Acerted = false
                            });
                        }
                    }
                }

                wordleViewModel.Juego[wordleViewModel.ModoId.ToString()].Add(new Session() { 
                    Intento = wordleViewModel.Intentos,
                    WordInsert = wordleViewModel.PalabraIngresada,
                    Letters = letters
                });

                if (wordleViewModel.Intentos == 2)
                {
                    wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos].Pista = PistaForUser(wordleViewModel.JuegoDictionaryJson, wordleViewModel.Length);

                    if (wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos].Pista)
                        wordleViewModel.Juego[wordleViewModel.ModoId.ToString()][wordleViewModel.Intentos].PistaDescripcion = (await _IWordleService.GetModeWordDetailsAsync(1)).Pista;  // ??
                }

                wordleViewModel.JuegoDictionaryJson = System.Text.Json.JsonSerializer.Serialize(wordleViewModel.Juego);

                wordleViewModel.Intentos++;

                // Si la palabra ingresada es correcta
                if (wordSecret.Equals(wordleViewModel.PalabraIngresada))
                {
                    wordleViewModel.Resultado = 1;
                    wordleViewModel.GameFinish = true;
                }
                else if (wordleViewModel.Intentos == 5)
                {
                    wordleViewModel.Resultado = 0;
                    wordleViewModel.GameFinish = true;
                }

                // Acá guardaria en la cookie el json
                SaveGameInCookie(wordleViewModel.Juego, wordleViewModel.ModoId);

                return Json(wordleViewModel);
            }
            else
            {
                return Json(new { error = true, message = "La palabra ingresada no existe." });
            }

        }

        public bool PistaForUser(string juegoDictionaryJson, int length)
        {
            Dictionary<string, List<Session>> juego;

            try
            {
                juego = JsonConvert.DeserializeObject<Dictionary<string, List<Session>>>(juegoDictionaryJson);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error en JSON.", ex);
            }

            // Letras acertadas únicas agrupadas por posición
            var letrasAcertadasUnicas = juego
                .SelectMany(juegoEntry => juegoEntry.Value)
                .SelectMany(sesion => sesion.Letters
                    .Select(
                        (letra, index) => new { 
                            Letra = letra, Index = index, sesion.Intento 
                        }
                    )
                )
                .Where(x => x.Letra.Acerted)
                .GroupBy(x => new { x.Index, x.Letra.Letra })
                .Select(g => g.First())
                .ToList();

            return EsNecesariaPista(letrasAcertadasUnicas.Count, length);
        }

        public bool EsNecesariaPista(int cantidadLetrasAcertadas, int wordLength)
        {
            bool debeMostrarPista = false;

            switch (wordLength)
            {
                case 5:
                    if (cantidadLetrasAcertadas <= 3)
                        debeMostrarPista = true;
                    break;
                case 7:
                    if (cantidadLetrasAcertadas <= 4)
                        debeMostrarPista = true;
                    break;
            }

            return debeMostrarPista;
        }

        [HttpGet]
        public async Task<IActionResult> Result(int result, int intento, int modoId, string modoDescripcion)
        {
            var aleatoriaDTO = await _IWordleService.GetModeWordDetailsAsync(modoId);

            var resultado = new ResultViewModel()
            {
                Intento = result == 1 ? ResultadoSegunIntento(intento) : "Que mal..",
                Palabra = aleatoriaDTO.Palabra,
                Definicion = aleatoriaDTO.Definicion,
                Sinonimos = aleatoriaDTO.Sinonimos,
                Antonimos = aleatoriaDTO.Antonimos,
                PalabraEn = aleatoriaDTO.PalabraEn,
                EjemploUso = aleatoriaDTO.EjemploUso,
                ModoDescripcion = modoDescripcion,
                Result = result == 1 ? 1 : 0
            };

            return PartialView("_Result", resultado);
        }

        [HttpGet]
        public IActionResult Instrucciones()
        {
            return PartialView("_Instrucciones");
        }

        private string CleanWord(string word)
        {
            var replacements = new Dictionary<char, char>
            {
                {'á', 'a'}, {'é', 'e'}, {'í', 'i'}, {'ó', 'o'}, {'ú', 'u'}, {'ä', 'a'}, {'ë', 'e'}, {'ï', 'i'}, {'ü', 'u'}, {'ö', 'o'},
                {'Á', 'A'}, {'É', 'E'}, {'Í', 'I'}, {'Ó', 'O'}, {'Ú', 'U'}, {'Ä', 'A'}, {'Ë', 'E'}, {'Ï', 'I'}, {'Ü', 'U'}, {'Ö', 'O'}
            };

            return new string(word.Select(c => replacements.ContainsKey(c) ? replacements[c] : c).ToArray());
        }

        private string ResultadoSegunIntento(int intento)
        {
            var result = string.Empty;

            switch (intento)
            {
                case 1:
                    result = EstadoResult.Excelente.ToString();
                    break;
                case 2:
                    result = EstadoResult.Buenisimo.ToString();
                    break;
                case 3:
                    result = EstadoResult.Aceptable.ToString();
                    break;
                case 4:
                    result = EstadoResult.Normal.ToString();
                    break;
                case 5:
                    result = EstadoResult.Mejorable.ToString();
                    break;
            }

            return result;
        }

        private enum EstadoResult
        {
            Excelente, Buenisimo, Aceptable, Normal, Mejorable
        }

        #region Cookies

        public void SaveGameInCookie(Dictionary<string, List<Session>> juegoDictionary, int modoId)
        {
            try
            {
                var listSession = juegoDictionary[modoId.ToString()];
                var listSessionJson = JsonConvert.SerializeObject(listSession);

                Response.Cookies.Append($"GameByModo_{modoId}", listSessionJson, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar cookie: {ex.Message}");
            }
        }

        public List<Session> GetGameFromCookie(int modoId)
        {
            try
            {
                if (Request.Cookies.TryGetValue($"GameByModo_{modoId}", out var listSessionJson))
                {
                    var listSession = JsonConvert.DeserializeObject<List<Session>>(listSessionJson);
                    return listSession;
                }

                return new List<Session>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recuperar la cookie: {ex.Message}");
            }
        }

        #endregion

    }
}
