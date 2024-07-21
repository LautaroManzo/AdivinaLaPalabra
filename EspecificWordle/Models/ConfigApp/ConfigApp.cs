
namespace EspecificWordle.Models.ConfigApp
{
    public class ConfigApp
    {
        #region RandomWord

        public string RandomWord { get; set; }

        public string RandomWordEn { get; set; }

        public string RandomWordDef { get; set; }
        
        public string RandomWordDefRae { get; set; }

        public List<string> RandomWordSynonyms { get; set; }

        public List<string> RandomWordAntonyms { get; set; }      
        
        public string RandomWordUseExamples { get; set; }        

        #endregion RandomWord
    }
}
