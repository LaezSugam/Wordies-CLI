namespace Wordies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    public static class StringToWordConverter
    {
        private static int _minimumLength = 1;
        private static int _maximumLength = 31;

        public static void RunConverter()
        {
            var path = Directory.GetCurrentDirectory() + "\\wordLists\\";

            for(var i = _minimumLength; i <= _maximumLength; i++)
            {
                List<Word> words = new List<Word>();
                var tempPath = path + i + "letterWords.txt";

                if(!File.Exists(tempPath))
                {
                    continue;
                }

                foreach(string line in File.ReadLines(tempPath))
                {
                    var word = new Word()
                    {
                        StringValue = line.ToUpper(),
                        Difficulty = 3,
                        Language = Word.Languages.EN_US,
                        Notes = string.Empty,
                    };

                    words.Add(word);
                }

                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(words, jsonOptions);

                var savePath = path + i + "letterWords.wrds";

                Console.WriteLine("Writing " + savePath);

                File.WriteAllText(savePath, jsonString);
            }
        }
    }
}