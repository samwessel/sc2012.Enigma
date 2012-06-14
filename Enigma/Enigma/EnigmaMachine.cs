using System;

namespace Enigma
{
    public class EnigmaMachine
    {
        private readonly IAlphabet _alphabet;
        private readonly IRotor _leftRotor;
        private readonly IRotor _centerRotor;
        private readonly IRotor _rightRotor;
        private readonly IReflector _reflector;

        public EnigmaMachine(IAlphabet alphabet, IRotor leftRotor, IRotor centerRotor, IRotor righRotor, IReflector reflector)
        {
            _alphabet = alphabet;
            _leftRotor = leftRotor;
            _centerRotor = centerRotor;
            _rightRotor = righRotor;
            _reflector = reflector;
        }

        public char Encode(char inputCharacter)
        {
            var initialOffset = _alphabet.IndexOf(inputCharacter);
            var rightRotorOffset = _rightRotor.EncodeRightToLeft(initialOffset);
            var centerRotorOffset = _centerRotor.EncodeRightToLeft(rightRotorOffset);
            var leftRotorOffset = _leftRotor.EncodeRightToLeft(centerRotorOffset);
            var reflectedOffset = _reflector.Reflect(leftRotorOffset);
            var returnedLeftOffset = _leftRotor.EncodeLeftToRight(reflectedOffset);
            var returnedCenterOffset = _centerRotor.EncodeLeftToRight(returnedLeftOffset);
            var returnedRightOffset = _rightRotor.EncodeLeftToRight(returnedCenterOffset);
            return _alphabet.CharacterAt(returnedRightOffset);
        }
    }
}