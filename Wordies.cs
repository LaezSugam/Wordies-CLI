namespace Wordies
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    class Wordies
    {
        private const int defaultAvailableGuesses = 10;
        private const int defaultNumberOfWordsToGuess = 5;

        private List<string> _words = new List<string>();

        private int _minimumLength = 5;
        private int _maximumLength = 5;
        private int _guessesAvailable = 6;
        private int _roundScore;
        private int _overallScore;
        private bool _wordleMode = false;
        private bool _isHardcoreMode = false;
        private int _round = 0;
        private int _guessesUsed = 0;
        private int _numberOfWordsToGuess = defaultNumberOfWordsToGuess;

        private int _roundPointValue 
        {
            get { return Math.Max(1, 5 - _round) * 100;}
        }

        private string _reportedWordsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Wordies Reported Words.txt";

        public Wordies()
        {

        }

        public void Run()
        {
            GameSetup();

            GameLoop();
        }

        private void GameSetup()
        {
            var useDoubleLetters = true;
            var path = Directory.GetCurrentDirectory() + "\\wordLists\\";

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            SetOptions();

            _words = GetWords(path, useDoubleLetters);
        }

        private void SetOptions()
        {
            GetGameType();

            CheckHardcoreEnabled();

            if(_wordleMode)
            {
                return;
            }

            GetMinimumWordLength();

            GetMaximumWordLength();

            SetGuesses();
        }

        private void GetGameType()
        {
            Console.WriteLine("Select Game Type: Type 1 for Wordies. Type 2 for Wordle.");

            if(Console.ReadLine() == "2")
            {
                _wordleMode = true;
            }
        }

        private void CheckHardcoreEnabled()
        {
            Console.WriteLine("With hardcore mode enabled, if you guess a letter that is in the word or correct");
            Console.WriteLine("you must use that letter in all subsequent guesses.");
            Console.WriteLine("Enable hardcore mode? (Y/N):");

            if(Console.ReadLine().ToUpper() == "Y")
            {
                _isHardcoreMode = true;
            }

            Console.Clear();

            if(_isHardcoreMode)
            {
                Console.WriteLine("Hardcore mode enabled.");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
        }

        private void GetMinimumWordLength()
        {
            Console.WriteLine("Enter minimum word length.");
            Console.WriteLine("Press ENTER without a length for default length (default is 5)");
            var readLineValue = Console.ReadLine();
            int readLength;
            
            if(readLineValue != "" && Int32.TryParse(readLineValue, out readLength))
            {
                _minimumLength = readLength;
            }

            Console.Clear();
        }

        private void GetMaximumWordLength()
        {
            Console.WriteLine("Enter maximum word length.");
            Console.WriteLine("Press ENTER without a length for default length (default is 5)");
            var readLineValue = Console.ReadLine();
            int readLength;
            
            if(readLineValue != "" && Int32.TryParse(readLineValue, out readLength))
            {
                _maximumLength = readLength;
            }

            if(_maximumLength < _minimumLength)
            {
                _maximumLength = _minimumLength;
            }

            Console.Clear();
        }

        private void SetGuesses()
        {
            _guessesAvailable = defaultAvailableGuesses;
        }

        private void GameLoop()
        {
            var keepPlaying = true;

            while(keepPlaying)
            {
                keepPlaying = false;
                var correctCharacters = 0;

                var rand = new Random();
                var wordToGuess = _words[rand.Next(_words.Count)].ToUpper();
                // wordToGuess = "BRASS";
                var guessedWords = new List<Guess>();
                var keyboard = new Keyboard();
                string guessSquares = "";

                for(var i = 0; i < wordToGuess.Length; i++)
                {
                    guessSquares += "\u25A1 ";
                }

                Console.WriteLine("W-O-R-D-I-E-S");

                Console.WriteLine("Debugging. Word is: " + wordToGuess);

                var answerIsCorrect = false;

                Console.WriteLine("Guess your word:");
                Console.WriteLine(guessSquares);

                Guess previousGuess = null;

                while(!answerIsCorrect && _guessesUsed < _guessesAvailable)
                {               
                    var answer = Console.ReadLine().ToUpper();
                    Console.Clear();
                    var verifyGuess = true;

                    if(answer.Length != wordToGuess.Length)
                    {
                        verifyGuess = false;
                        Console.WriteLine("Your answer must contain " + wordToGuess.Length + " letters.");
                        PrintGameScreen(keyboard, guessedWords);
                        continue;
                    }

                    var guessedWord = new Guess(answer, wordToGuess);
                    

                    // Send the whole guess words list in, dummy. Read the chars from that and copy that charguess into the keyboard.
                    // Keep the one that's most correct.
                    
                    if(_isHardcoreMode && !guessedWord.UsedRequiredLetters(previousGuess))
                    {
                        verifyGuess = false;
                        Console.WriteLine("All letters from previous guess that are correct or in the word must be used in your next guess.");
                    }

                    if(verifyGuess)
                    {
                        if(answer.ToUpper() == wordToGuess.ToUpper())
                        {
                            Console.WriteLine("You got it! Congratulations!");
                            answerIsCorrect = true;
                            UpdateScore(guessedWord, correctCharacters);
                            _roundScore = _roundScore * GetRoundScoreMultiplier();
                            _overallScore += _roundScore;

                            if(!_wordleMode)
                            {
                                _guessesAvailable += Math.Max(0, 3 - _round);
                                _numberOfWordsToGuess --;
                            }
                            
                            correctCharacters = guessedWord.CorrectCharacters;
                            guessedWords.Add(guessedWord);
                            keyboard.Update(guessedWord);
                        }
                        else if(!_words.Contains(answer))
                        {
                            Console.WriteLine("Word not in list, try again.");
                        }
                        else if(answer.Length != wordToGuess.Length)
                        {
                            Console.WriteLine("Incorrect number of characters, try again.");
                        }
                        else
                        {
                            UpdateScore(guessedWord, correctCharacters);
                            correctCharacters = guessedWord.CorrectCharacters;
                            _round++;
                            _guessesUsed++;
                            guessedWords.Add(guessedWord);
                            previousGuess = guessedWord;
                        
                            keyboard.Update(guessedWord);
                            if(_guessesUsed >= _guessesAvailable)
                            {
                                PrintGuesses(guessedWords);

                                Console.WriteLine("Sorry, out of guesses.");
                                Console.WriteLine("The word was:");
                                foreach(char letter in wordToGuess)
                                {
                                    Console.Write(letter + " ");
                                }
                                Console.WriteLine("");
                                break;
                            }
                        }
                    }

                    PrintGameScreen(keyboard, guessedWords);
                }

                Console.WriteLine("Report word? Y/N");
                if(Console.ReadLine().ToUpper() == "Y")
                {
                    ReportWord(wordToGuess);
                }

                if(_wordleMode)
                {
                    GameEndScreen();
                    PrintReturns(2);
                    //If we're in Wordle mode we want to reset and pick a new word
                    Console.WriteLine("Play Again? Y/N");
                    if(Console.ReadLine().ToUpper() == "Y")
                    {
                        Console.Clear();
                        _roundScore = 0;
                        _overallScore = 0;
                        _round = 0;
                        _guessesUsed = 0;
                        correctCharacters = 0;
                        keepPlaying = true;   
                    }
                    continue;
                }

                //In Wordies mode this will be replace with
                // 1) Figure out how many guesses we used this round and if under a certain number add some bonus guess
                // 2) Allow the user to pick a letter from the current word that will be in the next word
                // 3) Save the current round score off into an overall score bucket and reset the current round score
                // Probably some other stuff
              
                if(_guessesUsed < _guessesAvailable && _numberOfWordsToGuess > 0)
                {
                    Console.Clear();
                    _round = 0;
                    _roundScore = 0;
                    correctCharacters = 0;
                    keepPlaying = true;
                    Console.WriteLine("Guesses remaining: " + (_guessesAvailable - _guessesUsed) + "     Words remaining: " + _numberOfWordsToGuess);
                }
                else
                {
                    GameEndScreen();

                    Console.WriteLine("Play Again? Y/N");
                    if(Console.ReadLine().ToUpper() == "Y")
                    {
                        Console.Clear();
                        _roundScore = 0;
                        _overallScore = 0;
                        _round = 0;
                        _guessesUsed = 0;
                        _guessesAvailable = defaultAvailableGuesses;
                        _numberOfWordsToGuess = defaultNumberOfWordsToGuess;
                        correctCharacters = 0;
                        keepPlaying = true;   
                    }
                }

            }
        }

        private void PrintGuesses(List<Guess> guesses)
        {
            foreach(var guess in guesses)
            {
                guess.printGuess();
            }
        }

        private void PrintReturns(int numOfReturns = 1)
        {
            for(var i = 0; i < numOfReturns; i++)
            {
                Console.WriteLine("");
            }
        }

        private void PrintScore()
        {
            Console.WriteLine("Score: " + _roundScore);
        }

        private void ReportWord(string reportedWord)
        {
            Console.WriteLine("Reporting to path: " + _reportedWordsPath);

            if(!File.Exists(_reportedWordsPath))
            {
                File.Create(_reportedWordsPath);
            }

            File.AppendAllText(_reportedWordsPath, reportedWord + Environment.NewLine);
        }

        private List<string> GetWords(string path, bool useDoubleLetters = true)
        {
            List<string> words = new List<string>();

            for(var i = _minimumLength; i <= _maximumLength; i++)
            {
                var tempPath = path + i + "letterWords.txt";

                foreach(string line in File.ReadLines(tempPath))
                {
                    if(!useDoubleLetters)
                    {
                        var duplicate = false;

                        foreach(char letter in line)
                        {
                            var count = 0;

                            foreach(char character in line)
                            {
                                if(character == letter)
                                {
                                    count++;
                                }
                            }

                            if(count > 1)
                            {
                                duplicate = true;
                                break;
                            }
                        }

                        if(duplicate)
                        {
                            continue;
                        }
                    }

                    if(line.Length < _minimumLength || line.Length > _maximumLength)
                    {
                        continue;
                    }

                    words.Add(line.ToUpper());
                }
            }


            return words;
        }

        private void UpdateScore(Guess guessedWord, int correctCharacters)
        {
            _roundScore += Math.Max((guessedWord.CorrectCharacters - correctCharacters), 0) * _roundPointValue;
        }

        private int GetRoundScoreMultiplier()
        {
            return Math.Max(1, 5 - _round);
        }

        private void GameEndScreen()
        {
            double endScoreMultiplier = 1.0;

            Console.Clear();

            Console.WriteLine("Score: " + _overallScore);

            PrintReturns();

            if(_numberOfWordsToGuess <= 0)
            {
                Console.WriteLine("Congratulations, you got all of the words correct!");
                
                endScoreMultiplier += .25;

                PrintReturns();
            }

            if(_isHardcoreMode)
            {
                Console.WriteLine("*Hardcore Bonus*");

                endScoreMultiplier += .5;

                PrintReturns();
            }

            var guessesRemaining = _guessesAvailable - _guessesUsed - 1;
            if(guessesRemaining > 0)
            {
                Console.WriteLine( "*" + guessesRemaining + " Guesses Remaining Bonus*");

                endScoreMultiplier += (.1 * guessesRemaining);

                PrintReturns();
            }

            _overallScore = (int) (_overallScore * endScoreMultiplier);

            Console.WriteLine("FINAL SCORE: " + _overallScore);

        }

        private void PrintGameScreen(Keyboard keyboard, List<Guess> guessedWords)
        {
            PrintScore();

            PrintReturns(2);

            PrintGuesses(guessedWords);

            PrintReturns(2);
            
            keyboard.Print();
        }
    }
}