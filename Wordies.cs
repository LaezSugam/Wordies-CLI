namespace Wordies
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    class Wordies
    {
        private List<string> _words = new List<string>();

        private int _minimumLength = 5;
        private int _maximumLength = 5;

        private bool _isHardcoreMode = false;

        public Wordies()
        {

        }

        public void Run()
        {
            var useDoubleLetters = true;
            var path = Directory.GetCurrentDirectory() + "\\wordLists\\primaryWordList.txt";

            foreach(string line in File.ReadLines(path))
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

                _words.Add(line.ToUpper());
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var keepPlaying = true;

            while(keepPlaying)
            {
                keepPlaying = false;

                var rand = new Random();
                var wordToGuess = _words[rand.Next(_words.Count)].ToUpper();
                // wordToGuess = "BRASS";
                var guesses = 6;
                var guessedWords = new List<Guess>();
                var keyboard = new Keyboard();

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
                

                Console.WriteLine("W-O-R-D-I-E-S");

                Console.WriteLine("Debugging. Word is: " + wordToGuess);

                var answerIsCorrect = false;
                // var guessClue = "_ _ _ _ _";

                // keyboard.Print();

                Console.WriteLine("Guess your word:");
                Console.WriteLine("\u25A1 \u25A1 \u25A1 \u25A1 \u25A1");

                Guess previousGuess = null;

                while(!answerIsCorrect && guesses > 0)
                {               
                    var answer = Console.ReadLine().ToUpper();
                    Console.Clear();
                    var guessedWord = new Guess(answer, wordToGuess);
                    var verifyGuess = true;

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
                            guessedWords.Add(guessedWord);
                            keyboard.Update(guessedWord);
                            // PrintGuesses(guessedWords);
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
                            // guessClue = "";
                            guesses--;
                            guessedWords.Add(guessedWord);
                            previousGuess = guessedWord;
                        
                            keyboard.Update(guessedWord);
                            if(guesses <= 0)
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

                    PrintReturns(3);

                    keyboard.Print();

                    PrintReturns(2);
                    
                    PrintGuesses(guessedWords);
                }

                Console.WriteLine("Play Again? Y/N");
                if(Console.ReadLine().ToUpper() == "Y")
                {
                    Console.Clear();
                    keepPlaying = true;
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

        private void PrintReturns(int numOfReturns)
        {
            for(var i = 0; i < numOfReturns; i++)
            {
                Console.WriteLine("");
            }
        }
    }
}