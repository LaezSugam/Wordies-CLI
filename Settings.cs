namespace Wordies
{
    using System;
    using System.IO;
    using System.Text.Json;

    class Settings
    {
        public int availableGuesses {get; set;}
        public int numberOfWordsToGuess {get; set;}

        public bool showAnswer {get; set;}

        private string _settingsPath
        {
            get { return Directory.GetCurrentDirectory() + "\\settings.json"; }
        }

        public Settings()
        {

        }

        public Settings ReadSettings()
        {
            Settings settings = new Settings();

            if(File.Exists(_settingsPath))
            {
                string jsonString = File.ReadAllText(_settingsPath);
                settings = JsonSerializer.Deserialize<Settings>(jsonString);

                // Console.WriteLine(jsonString);
            }

            return settings;
        }

        public Settings ReadOrDefaultSettings()
        {
            if(File.Exists(_settingsPath))
            {
                return ReadSettings();
            }

            var settings = new Settings();

            // Settings file didn't exist, create a default settings file
            settings.availableGuesses = 15;
            settings.numberOfWordsToGuess = 5;
            settings.showAnswer = false;
            settings.WriteSettings();

            return settings;
        }

        public void WriteSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);

            File.WriteAllText(_settingsPath, jsonString);

            // Console.WriteLine(jsonString);
        }
    }
}