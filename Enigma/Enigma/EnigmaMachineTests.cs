using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Enigma
{
    [TestFixture]
    public class RotorWithReflectorTests
    {
        private int _reflectedOffset;
        private Mock<IReflector> _mockReflector;
        private int _encodedOffset;
        private Mock<IAlphabet> _leftAlphabet;
        private Mock<IAlphabet> _rightAlphabet;
        private int _offset;
        private char _retreivedCharacter;

        [SetUp]
        public void GivenReflectorWhenEncodingOffset()
        {
            _offset = 6;

            _reflectedOffset = 11;
            _mockReflector = new Mock<IReflector>();
            _mockReflector.Setup(reflector => reflector.Reflect(It.IsAny<int>())).Returns(_reflectedOffset);

            _retreivedCharacter = 'Z';
            _rightAlphabet = new Mock<IAlphabet>();
            _rightAlphabet.Setup(alphabet => alphabet.CharacterAt(It.IsAny<int>())).Returns(_retreivedCharacter);

            _encodedOffset = 10;
            _leftAlphabet = new Mock<IAlphabet>();
            _leftAlphabet.Setup(alphabet => alphabet.IndexOf(It.IsAny<char>())).Returns(_encodedOffset);

            new Rotor(_leftAlphabet.Object, _rightAlphabet.Object, _mockReflector.Object).Encode(_offset);
        }

        [Test]
        public void RightAlphabetLooksUpCharacterAtOffset()
        {
            _rightAlphabet.Verify(alphabet => alphabet.CharacterAt(_offset));
        }

        [Test]
        public void LeftAlphabetFindsIndexOfCharacterAtRightOffset()
        {
            _leftAlphabet.Verify(alphabet => alphabet.IndexOf(_retreivedCharacter));
        }

        [Test]
        public void ThenReflectorReflectsEncodedOffset()
        {
            _mockReflector.Verify(reflector => reflector.Reflect(_encodedOffset));
        }
    }

    public class Rotor
    {
        private readonly IAlphabet _leftAlphabet;
        private readonly IAlphabet _rightAlphabet;
        private readonly IReflector _reflector;

        public Rotor(IAlphabet leftAlphabet, IAlphabet rightAlphabet, IReflector reflector)
        {
            _leftAlphabet = leftAlphabet;
            _rightAlphabet = rightAlphabet;
            _reflector = reflector;
        }

        public void Encode(int offset)
        {
            var characterAt = _rightAlphabet.CharacterAt(offset);
            var encodedOffset = _leftAlphabet.IndexOf(characterAt);
            var reflectedOffset = _reflector.Reflect(encodedOffset);
        }
    }

    [TestFixture]
    public class EnigmaMachineTests
    {
        private char _inputCharacter;
        private Mock<IAlphabet> _mockAlphabet;
        private Mock<IRotor> _rightRotor;
        private int _indexOfInputCharacter;
        private Mock<IReflector> _mockReflector;
        private int _rightRotorEncodedOffset;
        private int _reflectedOffset;
        private int _rightRotorReturnedOffset;
        private char _result;
        private char _character;
        private Mock<IRotor> _centerRotor;
        private int _centerRotorEncodedOffset;
        private int _centerRotorReturnedOffset;
        private int _leftRotorEncodedOffset;
        private int _leftRotorReturnedOffset;
        private Mock<IRotor> _leftRotor;

        [SetUp]
        public void GivenOneRotorAndReflectorWhenEncodingSingleCharacter()
        {
            _inputCharacter = 'P';

            _indexOfInputCharacter = 123;
            _character = 'W';
            _mockAlphabet = new Mock<IAlphabet>();
            _mockAlphabet.Setup(alphabet => alphabet.IndexOf(It.IsAny<char>())).Returns(_indexOfInputCharacter);
            _mockAlphabet.Setup(alphabet => alphabet.CharacterAt(It.IsAny<int>())).Returns(_character);

            _rightRotorEncodedOffset = 321;
            _rightRotorReturnedOffset = 12345;

            _rightRotor = new Mock<IRotor>();
            _rightRotor.Setup(x => x.EncodeRightToLeft(It.IsAny<int>())).Returns(_rightRotorEncodedOffset);
            _rightRotor.Setup(x => x.EncodeLeftToRight(It.IsAny<int>())).Returns(_rightRotorReturnedOffset);

            _centerRotorEncodedOffset = 324361;
            _centerRotorReturnedOffset = 1234645;

            _centerRotor = new Mock<IRotor>();
            _centerRotor.Setup(x => x.EncodeRightToLeft(It.IsAny<int>())).Returns(_centerRotorEncodedOffset);
            _centerRotor.Setup(x => x.EncodeLeftToRight(It.IsAny<int>())).Returns(_centerRotorReturnedOffset);

            _leftRotorEncodedOffset = 8;
            _leftRotorReturnedOffset = 3;

            _leftRotor = new Mock<IRotor>();
            _leftRotor.Setup(x => x.EncodeRightToLeft(It.IsAny<int>())).Returns(_leftRotorEncodedOffset);
            _leftRotor.Setup(x => x.EncodeLeftToRight(It.IsAny<int>())).Returns(_leftRotorReturnedOffset);
            
            _reflectedOffset = 11;
            _mockReflector = new Mock<IReflector>();
            _mockReflector.Setup(reflector => reflector.Reflect(It.IsAny<int>())).Returns(_reflectedOffset);

            var enigmaMachine = new EnigmaMachine(_mockAlphabet.Object, _leftRotor.Object, _centerRotor.Object, _rightRotor.Object, _mockReflector.Object);
            _result = enigmaMachine.Encode(_inputCharacter);
        }

        [Test]
        public void ThenItGetsTheIntegerOffsetInTheAlphabet()
        {
            _mockAlphabet.Verify(alphabet => alphabet.IndexOf(_inputCharacter));
        }

        [Test]
        public void ThenItPassesTheOffsetIntoTheRightRotor()
        {
            _rightRotor.Verify(rotor => rotor.EncodeRightToLeft(_indexOfInputCharacter));
        }

        [Test]
        public void ThenItPassesTheRightOffsetIntoTheCenterRotor()
        {
            _centerRotor.Verify(rotor => rotor.EncodeRightToLeft(_rightRotorEncodedOffset));
        }

        [Test]
        public void ThenItPassesTheCenterOffsetIntoTheLeftRotor()
        {
            _leftRotor.Verify(rotor => rotor.EncodeRightToLeft(_centerRotorEncodedOffset));
        }
        
        [Test]
        public void ThenItPassesTheEncodedOffsetToTheReflector()
        {
            _mockReflector.Verify(reflector => reflector.Reflect(_leftRotorEncodedOffset));
        }

        [Test]
        public void ThenItPassesTheReflectedOffsetTheOtherWayThroughTheLeftRotor()
        {
            _leftRotor.Verify(rotor => rotor.EncodeLeftToRight(_reflectedOffset));
        }

        [Test]
        public void ThenItPassesTheLeftRotorReturnedOffsetTheOtherWayThroughTheCenterRotor()
        {
            _centerRotor.Verify(rotor => rotor.EncodeLeftToRight(_leftRotorReturnedOffset));
        }

        [Test]
        public void ThenItPassesTheCenterRotorReturnedOffsetTheOtherWayThroughTheRightRotor()
        {
            _rightRotor.Verify(rotor => rotor.EncodeLeftToRight(_centerRotorReturnedOffset));
        }

        [Test]
        public void ThenItPassesTheReturnedOffsetToTheAlphabet()
        {
            _mockAlphabet.Verify(alphabet => alphabet.CharacterAt(_rightRotorReturnedOffset));
        }

        [Test]
        public void ThenItReturnsTheReturnedOffsetCharacterFromTheAlphabet()
        {
            Assert.That(_result, Is.EqualTo(_character));
        }
    }

    public interface IReflector
    {
        int Reflect(int encondedOffset);
    }

    public interface IRotor
    {
        int EncodeRightToLeft(int offset);
        int EncodeLeftToRight(int offset);
    }

    public interface IAlphabet
    {
        int IndexOf(char inputCharacter);
        char CharacterAt(int returnedOffset);
    }
}
