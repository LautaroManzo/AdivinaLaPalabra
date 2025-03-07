﻿using DataBase.Models;

namespace EspecificWordle.DTOs
{
    public class WordDTO
    {
        public int ModeId { get; set; }

        public string Palabra { get; set; }

        public string Definicion { get; set; }

        public string PalabraEn { get; set; }

        public string Pista { get; set; }

        public string EjemploUso { get; set; }

        public List<Sinonimo> Sinonimos { get; set; }

        public List<Antonimo> Antonimos { get; set; }
    }
}
