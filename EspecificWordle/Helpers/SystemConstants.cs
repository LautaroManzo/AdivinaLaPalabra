namespace EspecificWordle.Helpers
{
    public class SystemConstants
    {

        public static string CleanWord(string word)
        {
            var replacements = new Dictionary<char, char>
            {
                {'á', 'a'}, {'é', 'e'}, {'í', 'i'}, {'ó', 'o'}, {'ú', 'u'}, {'ä', 'a'}, {'ë', 'e'}, {'ï', 'i'}, {'ü', 'u'}, {'ö', 'o'},
                {'Á', 'A'}, {'É', 'E'}, {'Í', 'I'}, {'Ó', 'O'}, {'Ú', 'U'}, {'Ä', 'A'}, {'Ë', 'E'}, {'Ï', 'I'}, {'Ü', 'U'}, {'Ö', 'O'}
            };

            return new string(word.Select(c => replacements.ContainsKey(c) ? replacements[c] : c).ToArray());
        }

        public struct EstadoResult
        {
            public const string Excelente = "Excelente";
            public const string Buenisimo = "Buenisimo";
            public const string Aceptable = "Aceptable";
            public const string Normal = "Normal";
            public const string Mejorable = "Mejorable";
        }

        public struct ColorLetra
        {
            public const string Gris = "Gris";
            public const string Amarillo = "Amarillo";
            public const string Verde = "Verde";
        }

    }
}
