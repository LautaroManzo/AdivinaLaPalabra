﻿namespace EspecificWordle.Models.Wordle
{
    public class ResultViewModel
    {
        public string Intento { get; set; }

        public string Palabra { get; set; }

        public string Definicion { get; set; }

        public string DefinicionRae { get; set; }

        public string Sinonimos { get; set; }

        public string Antonimos { get; set; }

        public string PalabraEn { get; set; }

        public string EjemploUso { get; set; }

        public bool Result { get; set; }
    }
}