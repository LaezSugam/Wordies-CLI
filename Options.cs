namespace Wordies
{
    using System;
    using System.IO;
    using System.Text.Json;

    class Options
    {
        public int minimumWordLength {get; set;}
        public int maximumWordLength {get; set;}

        public bool hardcore {get; set;}

        private string _optionsPath
        {
            get { return Directory.GetCurrentDirectory() + "\\options.json"; }
        }

        public Options()
        {

        }

        public Options ReadOptions()
        {
            Options options = new Options();

            if(File.Exists(_optionsPath))
            {
                string jsonString = File.ReadAllText(_optionsPath);
                options = JsonSerializer.Deserialize<Options>(jsonString);

                Console.WriteLine(jsonString);
            }

            return options;
        }

        public Options ReadOrDefaultOptions()
        {
            if(File.Exists(_optionsPath))
            {
                return ReadOptions();
            }

            var options = new Options();

            // Settings file didn't exist, create a default settings file
            options.maximumWordLength = 5;
            options.minimumWordLength = 5;
            options.hardcore = false;
            options.WriteOptions();

            return options;
        }

        public void WriteOptions()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, jsonOptions);

            File.WriteAllText(_optionsPath, jsonString);

            Console.WriteLine(jsonString);
        }
    }
}