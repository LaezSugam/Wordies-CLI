namespace Wordies
{
    class Score
    {
        public string Name {get; set;}
        public int Points {get; set;}
        
        public Score()
        {
            
        }

        public Score(string name, int points)
        {
            Name = name;
            Points = points;
        }
    }
}