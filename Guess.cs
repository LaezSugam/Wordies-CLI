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
                _charGuesses.Add(new CharGuess(guess[i]));
            }

            VerifyGuess();
        }

        private void VerifyGuess()
        {
            for(int i = 0; i < Answer.Length; i++)
            {
                var charGuess = _charGuesses[i];
                charGuess.BeenGuessed = true;

                if(charGuess.GuessedChar == Answer[i])
                {
                    charGuess.IsCorrect = true;
                    charGuess.IsInWord = true;
                }

                _charGuesses[i] = charGuess;
            }

            for(int i = 0; i < Answer.Length; i++)
            {
                var charGuess = _charGuesses[i];

                if(charGuess.GuessedChar == Answer[i])
                {
                    continue;
                }
                else if(Answer.Contains(charGuess.GuessedChar))
                {
                    var charactersInWord = Answer.Where(c => c == charGuess.GuessedChar);
                    var guessedChars = _charGuesses.Where(c => c.GuessedChar == charGuess.GuessedChar);

                    if(charactersInWord.Count() == 1 && !guessedChars.Any(c => c.IsCorrect) ||
                    charactersInWord.Count() > 1 && !guessedChars.All(c => c.IsCorrect))
                    {
                        charGuess.IsInWord = true;
                    }
                }

                _charGuesses[i] = charGuess;
            }
        }

        public bool IsCharacterCorrect(char character)
        {
            return _charGuesses.Where(c => c.GuessedChar == character).Any(c => c.IsCorrect);
        }

        public CharGuess GetBestCharGuess(char character)
        {
            CharGuess guess = _charGuesses.FirstOrDefault(c => c.GuessedChar == character && c.IsCorrect);
            
            if(guess == null)
            {
                guess = _charGuesses.FirstOrDefault(c => c.GuessedChar == character && c.IsInWord);
            }

            if(guess == null)
            {
                guess = _charGuesses.FirstOrDefault(c => c.GuessedChar == character);
            }

            return guess;
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
