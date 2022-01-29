using System;
using System.Collections.Generic;

namespace Wordies
{
    public class CharGuess
    {
        private char _playerGuess;

        public bool BeenGuessed;

        public char GuessedChar
        {
            get{ return _playerGuess;}
        }

        public bool IsCorrect;

        public bool IsInWord;

        public CharGuess(char guess)
        {
            _playerGuess = guess;
        }

        public bool IsBetterGuess(CharGuess guess)
        {
            if(IsCorrect)
            {
                return true;
            }
            else if(IsInWord && !guess.IsCorrect)
            {
                return true;
            }
            else if(BeenGuessed && !guess.IsInWord)
            {
                return true;
            }

            return false;
        }
    }
}
