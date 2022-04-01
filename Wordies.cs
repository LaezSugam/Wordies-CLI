namespace Wordies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    class Wordies
    {
        private List<string> _words = new List<string>();

        private Settings _settings = new Settings().ReadOrDefaultSettings();
        private Options _options = new Options().ReadOrDefaultOptions();

        private Scoreboard _scoreboard = new Scoreboard().ReadOrDefaultScoreboard();

        private int _minimumLength = 5;
        private int _maximumLength = 5;
        private int _guessesAvailable = 6;
        private int _roundScore;
        private int _overallScore;
        private bool _wordleMode = false;
        private bool _isHardcoreMode = false;
        private int _round = 0;
        private int _guessesUsed = 0;
        private int _numberOfWordsToGuess;
        private char _carryOverCharacter = ' ';

        private int _roundPointValue 
        {
            get { return Math.Max(1, 5 - _round) * 100;}
        }

        private string _reportedWordsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Wordies Reported Words.txt";

        public Wordies()
        {
            _numberOfWordsToGuess = _settings.numberOfWordsToGuess;
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

            SetUpGame();

            _words = GetWords(path, useDoubleLetters);
        }

        private void SetUpGame()
        {
            while(true)
            {
                Console.WriteLine("Type 1 to play Wordies. Type 2 to play Wordle. Type 3 for Options.");
                var input = Console.ReadLine();

                if(input == "2" || input.ToUpper() == "WORDLE")
                {
                    _minimumLength = 5;
                    _maximumLength = 5;
                    _isHardcoreMode = _options.Hardcore;
                    _wordleMode = true;
                    break;
                }

                if(input == "3" || input.ToUpper() == "OPTIONS" || input.ToUpper() == "OPTION")
                {
                    _options.GetHardcoreEnabled();

                    _options.GetMinimumWordLength();

                    _options.GetMaximumWordLength();

                    _options.WriteOptions();

                    continue;
                }

                SetupFromOptions();
                SetGuesses();
                break;
            }
  
        }

        private void SetupFromOptions()
        {
            _minimumLength = _options.MinimumWordLength;
            _maximumLength = _options.MaximumWordLength;
            _isHardcoreMode = _options.Hardcore;
        }

        

        private void SetGuesses()
        {
            _guessesAvailable = _settings.availableGuesses;
        }

        private void GameLoop()
        {
            var keepPlaying = true;

            while(keepPlaying)
            {
                keepPlaying = false;
                var correctCharacters = 0;

                var rand = new Random();
                var words = _words;

                if(_carryOverCharacter != ' ')
                {
                    words = words.Where(w => w.ToUpper().Contains(_carryOverCharacter)).ToList();
                }

                var wordToGuess = words[rand.Next(words.Count)].ToUpper();
                var guessedWords = new List<Guess>();
                var keyboard = new Keyboard();

                if(_carryOverCharacter != ' ')
                {
                    var initialCharacterGuess = new Guess(string.Concat(Enumerable.Repeat(_carryOverCharacter, wordToGuess.Length)), wordToGuess);
                    keyboard.Update(initialCharacterGuess);
                }

                string guessSquares = "";

                for(var i = 0; i < wordToGuess.Length; i++)
                {
                    if(wordToGuess[i] == _carryOverCharacter)
                    {
                        guessSquares += _carryOverCharacter + " ";
                        _carryOverCharacter = ' ';
                    }
                    else
                    {
                        guessSquares += "\u25A1 ";
                    }  
                }

                Console.WriteLine("W-O-R-D-I-E-S");

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
                        PrintGameScreen(keyboard, guessedWords, wordToGuess);
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
                            _roundScore = (int) (_roundScore * GetRoundScoreMultiplier(wordToGuess.Length));
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

                    PrintGameScreen(keyboard, guessedWords, wordToGuess);
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
                    GetCarryoverCharacter(wordToGuess);
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
                        _guessesAvailable = _settings.availableGuesses;
                        _numberOfWordsToGuess = _settings.numberOfWordsToGuess;
                        correctCharacters = 0;
                        keepPlaying = true;   
                    }
                }

            }
        }

        private void GetCarryoverCharacter(string wordToGuess)
        {
            Console.Clear();

            Console.WriteLine("Enter a letter from the current word to carry over to next word.");
            Console.WriteLine("Press ENTER without a letter to continue without carrying a letter forward.");
            Console.WriteLine("Current word is: " + wordToGuess);
            var readLine = Console.ReadLine();
            var carryOverChar = readLine != string.Empty ? readLine.ToUpper()[0] : ' ';

            if(wordToGuess.Contains(carryOverChar))
            {
                _carryOverCharacter = carryOverChar;
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

        private double GetRoundScoreMultiplier(int wordLength)
        {
            double lengthMultiplier = (wordLength - 5) * .1 + 1;

            return Math.Max(1, 5 - _round) * lengthMultiplier;
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

                endScoreMultiplier += 1;

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

            var isNewHighScore = false;

            if(_wordleMode)
            {
                isNewHighScore = _scoreboard.IsNewWordleHighScore(_overallScore);
            }
            else
            {
                isNewHighScore = _scoreboard.IsNewWordiesHighScore(_overallScore);
            }

            if(isNewHighScore)
            {
                PrintReturns(2);
                Console.WriteLine("CONGRATULATIONS. You have a new high score! Please enter initials.");

                var input = Console.ReadLine().ToUpper();

                input = input.Length > 3 ? input.Substring(0, 3) : input;

                if(_wordleMode)
                {
                    _scoreboard.UpdateWordleHighScores(_overallScore, input);
                    _scoreboard.PrintWordleScores();
                }
                else
                {
                    _scoreboard.UpdateWordiesHighScores(_overallScore, input);
                    _scoreboard.PrintWordiesScores();
                }

                Console.WriteLine("Press ENTER to continue.");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Press ENTER to continue.");
                Console.ReadKey();

                if(_wordleMode)
                {
                    _scoreboard.PrintWordleScores();
                }
                else
                {
                    _scoreboard.PrintWordiesScores();
                }

                Console.WriteLine("Press ENTER to continue.");
                Console.ReadKey();
                Console.Clear();
            }

        }

        private void PrintGameScreen(Keyboard keyboard, List<Guess> guessedWords, string wordToGuess)
        {
            if(_settings.showAnswer)
            {
                Console.WriteLine("Debugging. Word is: " + wordToGuess);
                PrintReturns();
            }

            PrintScore();

            PrintReturns(2);

            PrintGuesses(guessedWords);

            PrintReturns(2);
            
            keyboard.Print();
        }
    }
}