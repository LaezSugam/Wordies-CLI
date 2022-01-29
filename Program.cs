using System;
using System.Collections.Generic;
using System.IO;

namespace Wordies
{
    class Program
    {
        static List<string> Words = new List<string>();

        private static int minimumLength = 5;
        private static int maximumLength = 5;

        static void Main(string[] args)
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

                if(line.Length < minimumLength || line.Length > maximumLength)
                {
                    continue;
                }

                Words.Add(line.ToUpper());
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var keepPlaying = true;

            while(keepPlaying)
            {
                keepPlaying = false;

                var rand = new Random();
                var wordToGuess = Words[rand.Next(Words.Count)].ToUpper();
                // wordToGuess = "BRASS";
                var guesses = 6;
                var guessedWords = new List<Guess>();
                var keyboard = new Keyboard();
                

                Console.WriteLine("W-O-R-D-I-E-S");

                // Console.WriteLine("Debugging. Word is: " + wordToGuess);

                var answerIsCorrect = false;
                // var guessClue = "_ _ _ _ _";

                // keyboard.Print();

                Console.WriteLine("Guess your word:");
                Console.WriteLine("\u25A1 \u25A1 \u25A1 \u25A1 \u25A1");

                while(!answerIsCorrect && guesses > 0)
                {               
                    var answer = Console.ReadLine().ToUpper();
                    Console.Clear();
                    var guessedWord = new Guess(answer, wordToGuess);

                    // Send the whole guess words list in, dummy. Read the chars from that and copy that charguess into the keyboard.
                    // Keep the one that's most correct.

                    if(answer.ToUpper() == wordToGuess.ToUpper())
                    {
                        Console.WriteLine("You got it! Congratulations!");
                        answerIsCorrect = true;
                        guessedWords.Add(guessedWord);
                        keyboard.Update(guessedWord);
                        // PrintGuesses(guessedWords);
                    }
                    else if(!Words.Contains(answer))
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
                        guessedWords.Add(new Guess(answer, wordToGuess));
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

                        // for(var i = 0; i < wordToGuess.Length; i++)
                        // {
                        //     if(wordToGuess[i] == answer[i])
                        //     {
                        //         // guessClue += wordToGuess[i] + " ";
                        //         Console.BackgroundColor = ConsoleColor.DarkGreen;
                        //     }
                        //     else if(wordToGuess.Contains(answer[i]))
                        //     {
                        //         Console.ForegroundColor = ConsoleColor.Black;
                        //         Console.BackgroundColor = ConsoleColor.DarkYellow;
                        //         // guessClue += '\u25A1' + " ";
                        //     }
                        //     else
                        //     {
                        //         Console.BackgroundColor = ConsoleColor.DarkGray;
                        //         // guessClue += "_ ";
                        //     }
                        //     Console.Write(answer[i]);
                        //     Console.ResetColor();
                        //     Console.Write(" ");
                        // }

                        

                        // Console.ResetColor();
                        // Console.WriteLine("");
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

        static void PrintGuesses(List<Guess> guesses)
        {
            foreach(var guess in guesses)
            {
                guess.printGuess();
            }
        }

        static void PrintReturns(int numOfReturns)
        {
            for(var i = 0; i < numOfReturns; i++)
            {
                Console.WriteLine("");
            }
        }
    }
}
