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

        public struct EstadoResultadoExitoso
        {
            public const string Impecable = "Impecable, qué nivel! 👌";
            public const string Experto = "La tenés clarísima! 😎";
            public const string Aceptable = "Bueno, no es para aplaudir, pero bien 👍";
            public const string Regular = "Medio pelo, pero bueno... 🤷‍";
            public const string Justo = "Casi te rendís 😅";
        }

        public struct EstadoResultadoFallido
        {
            public const string Vergonzoso = "Ni cerca... una vergüenza 😵‍";
            public const string Flojo = "Flojardi, muy flojardi... 😬";
            public const string DesastreTotal = "Un desastre, no hay otra palabra 🤦‍";
            public const string EraFacil = "No era tan difícil, che... 😐";
            public const string Perdido = "Perdido como turista en el bondi 🚌";
            public const string SobrePensado = "Te pasaste de rosca pensando";
            public const string SinPalabras = "Mejor no digo nada... qué papelón 😬";
            public const string MasOnda = "La próxima ponele mas onda";
        }

        public struct ColorLetra
        {
            public const string Gris = "Gris";
            public const string Amarillo = "Amarillo";
            public const string Verde = "Verde";
        }

    }
}
