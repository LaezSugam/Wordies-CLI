namespace Wordies
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Collections.Generic;

    class Scoreboard
    {
        public List<Score> WordiesHighScores {get; set;}
        public List<Score> InfiniteHighScores {get; set;}
        public List<Score> WordleHighScores {get; set;}


        private string _scoreboardPath
        {
            get { return Directory.GetCurrentDirectory() + "\\scoreboard.json"; }
        }

        public Scoreboard()
        {

        }

        public Scoreboard ReadScoreboard()
        {
            Scoreboard scoreboard = new Scoreboard();

            if(File.Exists(_scoreboardPath))
            {
                string jsonString = File.ReadAllText(_scoreboardPath);
                scoreboard = JsonSerializer.Deserialize<Scoreboard>(jsonString);

                // Console.WriteLine(jsonString);
            }

            return scoreboard;
        }

        public Scoreboard ReadOrDefaultScoreboard()
        {
            if(File.Exists(_scoreboardPath))
            {
                return ReadScoreboard();
            }

            Scoreboard scoreboard = new Scoreboard();

            // Settings file didn't exist, create a default settings file
            scoreboard.WordiesHighScores = new List<Score>();
            scoreboard.InfiniteHighScores = new List<Score>();
            scoreboard.WordleHighScores = new List<Score>();
            scoreboard.WriteScoreboard();

            return scoreboard;
        }

        public void WriteScoreboard()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true};
            string jsonString = JsonSerializer.Serialize(this, jsonOptions);

            File.WriteAllText(_scoreboardPath, jsonString);

            Console.WriteLine(jsonString);
        }

        public void PrintWordleScores()
        {
            PrintScores(WordleHighScores);
        }

        public void PrintWordiesScores()
        {
            PrintScores(WordiesHighScores);
        }

        public void PrintInfiniteScores()
        {
            PrintScores(InfiniteHighScores);
        }

        private void PrintScores(List<Score> scores)
        {
            Console.Clear();
            Console.WriteLine("HIGH SCORES");
            Console.WriteLine("");

            for(int i = 0; i < scores.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + scores[i].Name + "   " + scores[i].Points);
                Console.WriteLine("");
            }
        }

        public bool IsNewWordleHighScore(int score)
        {
            return IsNewHighScore(score, WordleHighScores);
        }

        public bool IsNewWordiesHighScore(int score)
        {
            return IsNewHighScore(score, WordiesHighScores);
        }

        public bool IsNewInfiniteHighScore(int score)
        {
            return IsNewHighScore(score, InfiniteHighScores);
        }

        private bool IsNewHighScore(int score, List<Score> highScores)
        {
            if(highScores.Count < 10 || highScores[highScores.Count - 1].Points < score)
            {
                return true;
            }

            return false;
        }

        public void UpdateWordleHighScores(int score, string name)
        {
            WordleHighScores = UpdateScoreboard(score, name, WordleHighScores);

            WriteScoreboard();
        }

        public void UpdateWordiesHighScores(int score, string name)
        {
            WordiesHighScores = UpdateScoreboard(score, name, WordiesHighScores);

            WriteScoreboard();
        }

        public void UpdateInfiniteHighScores(int score, string name)
        {
            InfiniteHighScores = UpdateScoreboard(score, name, InfiniteHighScores);

            WriteScoreboard();
        }

        private List<Score> UpdateScoreboard(int score, string name, List<Score> highScores)
        {
            highScores.Add(new Score(name, score));
            highScores.Sort((s1, s2) => s2.Points.CompareTo(s1.Points));

            if(highScores.Count > 10)
            {
                highScores = highScores.GetRange(0, 10);
            }

            return highScores;
        }
    }
}