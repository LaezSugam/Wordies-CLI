using System;
using System.Collections.Generic;
using System.Linq;

namespace Wordies
{
    class Guess
    {
        public string PlayerGuess;
        public string Answer;

        private List<CharGuess> _charGuesses = new List<CharGuess>();

        public Guess (string guess, string answer)
        {
            PlayerGuess = guess;
            Answer = answer;

            for(var i = 0; i < PlayerGuess.Length; i++)
            {
                _charGuesses.Add(new CharGuess(guess[i], answer[i], answer));
            }
        }

        public bool IsCharacterCorrect(char character)
        {
            return _charGuesses.Where(c => c.GuessedChar == character).Any(c => c.IsCorrect);
        }

        public CharGuess GetCharGuess(char character)
        {
            return _charGuesses.FirstOrDefault(c => c.GuessedChar == character);
        }

        public void printGuess()
        {
            foreach(var guess in _charGuesses)
            {
                if(guess.IsCorrect)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                }
                else if(guess.IsInWord)
                {
                    var countyCount = _charGuesses.Count(c => c.GuessedChar == guess.GuessedChar);
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                }

                Console.Write(guess.GuessedChar);
                Console.ResetColor();
                Console.Write(" ");
            }

            Console.WriteLine("");
        }

    }
}
