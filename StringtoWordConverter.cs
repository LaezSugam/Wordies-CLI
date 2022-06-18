namespace Wordies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    public static class StringToWordConverter
    {
        private static int _minimumLength = 3;
        private static int _maximumLength = 15;

        public static void RunConverter()
        {
            var path = Directory.GetCurrentDirectory() + "\\wordLists\\";

            for(var i = _minimumLength; i <= _maximumLength; i++)
            {
                List<Word> words = new List<Word>();
                var tempPath = path + i + "-letter-words.json";

                // List<Word> words = new List<Word>();
                // var tempPath = path + i + "letterWords.txt";

                // if(!File.Exists(tempPath))
                // {
                //     continue;
                // }

                // foreach(string line in File.ReadLines(tempPath))
                // {
                //     var word = new Word()
                //     {
                //         StringValue = line.ToUpper(),
                //         Difficulty = 2,
                //         Language = Word.Languages.EN_US,
                //         Notes = string.Empty,
                //     };

                //     words.Add(word);
                // }

                string jsonStringDeserialized = File.ReadAllText(tempPath);
                List<ScrabbleWord> notWords = JsonSerializer.Deserialize<List<ScrabbleWord>>(jsonStringDeserialized);

                foreach(var notWord in notWords)
                {
                    var word = new Word()
                    {
                        StringValue = notWord.word.ToUpper(),
                        Difficulty = 2,
                        Language = Word.Languages.EN_US,
                        Notes = string.Empty,
                    };

                    words.Add(word);
                }

                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string jsonStringSerialized = JsonSerializer.Serialize(words, jsonOptions);

                var savePath = path + i + "letterWords.wrds";

                Console.WriteLine("Writing " + savePath);

                File.WriteAllText(savePath, jsonStringSerialized);
            }
        }

        class ScrabbleWord
        {
            public string word {get; set;}

            public ScrabbleWord()
            {
            }
        }
    }
}