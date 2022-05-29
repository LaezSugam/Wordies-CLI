namespace Wordies
{
    public class Word
    {
        public enum Languages
        {
            EN_US,
            EN_UK,
            EN_AU,
            EN_IN,
            EN_IR,
            EN_WA,
            EN_NZ,
            OTHER,
            NONE
        }

        public string StringValue { get; set; }

        public int Difficulty { get; set; }

        public Languages Language { get; set; }

        public string Notes { get; set; }

        public Word()
        {

        }
    }
}