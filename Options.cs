namespace Wordies
{
    using System;
    using System.IO;
    using System.Text.Json;

    class Options
    {
        public int MinimumWordLength {get; set;}
        public int MaximumWordLength {get; set;}

        public bool Hardcore {get; set;}

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
            options.MaximumWordLength = 5;
            options.MinimumWordLength = 5;
            options.Hardcore = false;
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

        public void GetHardcoreEnabled()
        {
            Console.Clear();
            Console.WriteLine("HARDCORE MODE");
            PrintReturns();
            Console.WriteLine("With hardcore mode enabled, if you guess a letter that is in the word or correct");
            Console.WriteLine("you must use that letter in all subsequent guesses.");
            Console.WriteLine("Enable hardcore mode? (Y/N):");

            if(Console.ReadLine().ToUpper() == "Y")
            {
                Hardcore = true;
            }

            Console.Clear();

            if(Hardcore)
            {
                Console.WriteLine("Hardcore mode enabled.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
        }

        public void GetMinimumWordLength()
        {
            Console.WriteLine("MINIMUM WORD LENGTH - WORDIES ONLY");
            PrintReturns();
            Console.WriteLine("Current minimum word length: " + MinimumWordLength);
            Console.WriteLine("Minimum word length sets the smallest word that can generated for play. Smaller words are worth fewer points.");
            Console.WriteLine("Press ENTER without a length to keep current length of " + MinimumWordLength);
            Console.WriteLine("Minimum length must be 3 or greater.");
            var readLineValue = Console.ReadLine();
            int readLength;
            
            if(readLineValue != "" && Int32.TryParse(readLineValue, out readLength))
            {
                MinimumWordLength = readLength >= 3 ? readLength : 3;               
            }

            Console.Clear();
        }

        public void GetMaximumWordLength()
        {
            Console.WriteLine("MAXIMUM WORD LENGTH - WORDIES ONLY");
            PrintReturns();
            Console.WriteLine("Current maximum word length: " + MaximumWordLength);
            Console.WriteLine("Maximum word length sets the largest word that can generated for play. Larger words are worth more points.");
            Console.WriteLine("Press ENTER without a length to keep current length of " + MaximumWordLength);
            Console.WriteLine("Maximum length must be greater than or equal to the set minimum word length.");
            var readLineValue = Console.ReadLine();
            int readLength;
            
            if(readLineValue != "" && Int32.TryParse(readLineValue, out readLength))
            {
                MaximumWordLength = readLength >= MinimumWordLength ? readLength : MinimumWordLength;
            }

            if(MaximumWordLength > 22)
            {
                MaximumWordLength = 22;
            }

            Console.Clear();
        }

        private void PrintReturns(int numOfReturns = 1)
        {
            for(var i = 0; i < numOfReturns; i++)
            {
                Console.WriteLine("");
            }
        }
    }
}