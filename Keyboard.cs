using System;
using System.Collections.Generic;

namespace Wordies
{
    class Keyboard
    {
        private List<List<CharGuess>> _keyboardLines = new List<List<CharGuess>>();

        public Keyboard ()
        {
            List<string> strings = new List<string>();
            strings.Add("QWERTYUIOP");
            strings.Add(" ASDFGHJKL");
            strings.Add("  ZXCVBNM");

            foreach(var stringy in strings)
            {
                List<CharGuess> keyboardLine = new List<CharGuess>();
                for(var i = 0; i < stringy.Length; i++)
                {
                        keyboardLine.Add(new CharGuess(stringy[i]));
                }

                _keyboardLines.Add(keyboardLine);
            }

            
        }

        public void Update(Guess guess)
        {
            foreach(var keyboardLine in _keyboardLines)
            {
                for(int i = 0; i < keyboardLine.Count; i++)
                {
                    var guessedChar = guess.GetBestCharGuess(keyboardLine[i].GuessedChar); 

                    if(guessedChar != null && guessedChar.IsBetterGuess(keyboardLine[i]))
                    {
                        keyboardLine[i] = guessedChar;
                    }
                }
            }
        }

        public void Print()
        {
            foreach(var charGuesses in _keyboardLines)
            {
                foreach(var guess in charGuesses)
                {
                    if(guess.IsCorrect)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }
                    else if(guess.IsInWord)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if(guess.GuessedChar == ' ')
                    {
                        Console.ResetColor();
                    }
                    else if(guess.BeenGuessed)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.Write(guess.GuessedChar == ' ' ? "" : guess.GuessedChar.ToString());
                    Console.ResetColor();
                    Console.Write(" ");
                }

                Console.WriteLine("");
            }

        }
    }
}
