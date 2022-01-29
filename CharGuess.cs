using System;
using System.Collections.Generic;

namespace Wordies
{
    public class CharGuess
    {
        private char _playerGuess;
        private string _answer;
        private int _answerChar;

        private bool? _isInWord;

        public bool BeenGuessed
        {
            get{return _answer != "";} 
        }

        public char GuessedChar
        {
            get{ return _playerGuess;}
        }

        public bool IsCorrect{
            get
            {
                return _playerGuess == ' ' ? false : _playerGuess == _answerChar;
            }
        }

        public bool IsInWord
        {
            get
            {
                if(_isInWord != null)
                {
                    return (bool) _isInWord;
                }
                else
                {
                    _isInWord = _playerGuess == ' ' ? false : _answer.Contains(_playerGuess);
                    return (bool) _isInWord;
                }
            }
            set
            {
                _isInWord = value;
            }
        }

        public CharGuess(char guess, char answerChar, string answer)
        {
            _playerGuess = guess;
            _answer = answer;
            _answerChar = answerChar;
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
