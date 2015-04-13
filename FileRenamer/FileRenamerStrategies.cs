﻿using System;
using System.Text;
using System.Text.RegularExpressions;
namespace FileRenamer
{



    /// <summary>
    /// This interface encapsulates a strategy to rename a single file.
    /// </summary>
    public interface IFileRenamerStrategy
    {
        String RenameFile(FileMetaData FileName, int Position);
    }

    /// <summary>
    /// Just return the name unchanged.
    /// </summary>
    public class IdentityStrategy : IFileRenamerStrategy
    {

        string IFileRenamerStrategy.RenameFile(FileMetaData FileName, int Position)
        {
            return FileName.Name;
        }
    }

    /// <summary>
    /// Different behaviours for dealing with suffixes:
    ///   NameOnly: only modify the name
    ///   SuffixOnly: only modify the suffix
    ///   BothNameSuffix: modify anything in the filename, including the separator
    /// The separator is the last period "." found in a file name, if it has one.
    /// The suffix is anything after the separator and the name is anything before.
    /// If a file name does not have a separator then the name is the whole file name, and the suffix is empty
    /// </summary>
    public enum NameSuffixBehaviour
    {
        NameOnly,
        SuffixOnly,
        BothNameSuffix
    };

    /// <summary>
    /// A class to help deal with names and suffixes.
    /// This class is a template, the different behaviours will be given by subclasses
    /// </summary>
    abstract internal class NameSuffixHelper
    {
        protected string _text;
        protected bool _hasSeparator;
        protected string _name;
        protected string _suffix;
        protected int _suffixPos;
        // _workingString is not set by the constructor of this class.
        // Further subclasses should set this in their constructors.
        protected string _workingString;

        /// <summary>
        /// This method will be overriden by subclasses in order to compute the final string.
        /// </summary>
        /// <param name="Input">The name with which we need modify</param>
        /// <returns>String with name, suffix, and/or separator added to it, depending on which subclass is used</returns>
        abstract protected string FinalString(string Input);

        /// <summary>
        /// Where in the original text the suffix starts
        /// </summary>
        public int SuffixPos
        {
            get { return _suffixPos; }
        }

        /// <summary>
        /// Does the filename have a separator
        /// </summary>
        public bool HasSeparator
        {
            get { return _hasSeparator; }
        }

        /// <summary>
        /// The suffix of the file name
        /// </summary>
        public string Suffix
        {
            get { return _suffix; }
        }

        /// <summary>
        ///  The name part of the file name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The whole text of the file name
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
        }

        /// <summary>
        /// The separator string of the file name.
        /// It is "." if a file has a separator, otherwise it is empty.
        /// </summary>
        public string SeparatorString
        {
            get
            {
                if (_hasSeparator)
                {
                    return ".";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Text">The filename</param>
        /// <param name="Behaviour">What behaviour to use for inserting and overwriting</param>
        public NameSuffixHelper(string Text)
        {
            _text = Text;
            _hasSeparator = _text.Contains(".");
            int finalDotPos = _text.LastIndexOf(".");
            _suffix = _text.Substring(finalDotPos + 1);
            if (finalDotPos >= 0)
            {
                _name = _text.Substring(0, finalDotPos);
                _suffixPos = finalDotPos;
            }
            else
            {
                _name = _text;
                _suffixPos = _text.Length;
            }
        }

        /// <summary>
        /// Insert some text into the given position, with behaviour given by the constructor.
        /// If the position is greater the length then it appends.
        /// </summary>
        /// <param name="Position">Position to insert</param>
        /// <param name="Text">Text to insert</param>
        /// <returns>New filename</returns>
        public string Insert(int Position, string Text)
        {
            return FinalString(InsertOrAppend(_workingString, Position, Text));
        }

        /// <summary>
        /// Overwrite parts of the filename with the given text, at the given position.
        /// The name/suffix behaviour is determined by the constructor.
        /// If the text to overwrite overruns the original text then the length is extended.
        /// If the position is greater than the length of the text then it appends.
        /// </summary>
        /// <param name="Position">Position to overwrite</param>
        /// <param name="Text">Text to overwrite</param>
        /// <returns>New filename</returns>
        public string Overwrite(int Position, string Text)
        {
            return FinalString(OverwriteOrAppend(_workingString, Position, Text));
        }

        /// <summary>
        /// Internal method for inserting text or append if the position is greater than the input length.
        /// </summary>
        /// <param name="InputText">Inial text</param>
        /// <param name="Position">Position to insert</param>
        /// <param name="InsertText">Text to insert</param>
        /// <returns>String</returns>
        private string InsertOrAppend(string InputText, int Position, string InsertText)
        {
            return InputText.Insert(Math.Min(Position, InputText.Length), InsertText);
        }

        /// <summary>
        /// Internal method to perform the overwrite.
        /// </summary>
        /// <param name="InputText">Inital text to overwrite</param>
        /// <param name="Position">Position to overwrite</param>
        /// <param name="OverwriteText">Text we overwrite with</param>
        /// <returns>String</returns>
        private string OverwriteOrAppend(string InputText, int Position, string OverwriteText)
        {
            var stringBuilder = new StringBuilder(InputText);
            // Find the right amount of characters to remove.  If we are overwriting with a string longer than
            // what remains then we just append to the end.
            var lengthToRemove = Math.Min(OverwriteText.Length, Math.Max(0, InputText.Length - Position));
            if (lengthToRemove > 0)
            {
                stringBuilder.Remove(Position, lengthToRemove);
            }
            stringBuilder.Insert(Position, OverwriteText);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Remove Characters from the name
        /// </summary>
        /// <param name="FromPos">Deletion starts from this position</param>
        /// <param name="FromLeft">Is the leftmost position calculated from the start of the string(true) or the end?</param>
        /// <param name="ToPos">Deletion ends at this position</param>
        /// <param name="ToLeft">Is the rightmost position calculated from start of string(true) or the end?</param>
        /// <returns>Filename with characters removed</returns>
        public string RemoveCharacters(int FromPos, bool FromLeft, int ToPos, bool ToLeft)
        {
            string newName;
            int leftPos;
            int rightPos;
            int currentLength;

            currentLength = _workingString.Length;

            // Calculate leftmost character to delete
            if (FromLeft)
            {
                leftPos = FromPos;
            }
            else
            {
                leftPos = currentLength - FromPos;
            }

            // Calculate rightmost character to delete
            if (ToLeft)
            {
                rightPos = ToPos;
            }
            else
            {
                rightPos = currentLength - ToPos;
            }

            // Clamp left position
            if (leftPos < 0)
            {
                leftPos = 0;
            }

            // Clamp right position
            if (rightPos > currentLength)
            {
                rightPos = currentLength;
            }

            // Perform deletion
            if (rightPos - leftPos >= 0)
            {
                newName = _workingString.Remove(leftPos, rightPos - leftPos);
            }
            else
            {
                newName = _workingString;
            }

            // Get final string via subclasses
            return FinalString(newName);
        }
    }

    /// <summary>
    /// A NameSuffixHelper which only modifies the name of a filename.
    /// </summary>
    internal class NameHelper : NameSuffixHelper 
    { 
        public NameHelper(string Text) : base(Text)
        {
            _workingString = _name;
        }

        override protected string FinalString(string Input)
        {
            return Input + SeparatorString + _suffix;
        }
    }

    /// <summary>
    /// A NameSuffixHelper which only modifies the suffix of a filename.
    /// </summary>
    internal class SuffixHelper : NameSuffixHelper
    {
        public SuffixHelper(string Text)
            : base(Text)
        {
            _workingString = _suffix;
        }

        override protected string FinalString(string Input)
        {
            return _name + SeparatorString + Input;
        }
    }

    /// <summary>
    /// A NameSuffixHelper which modifies any part of a filename.
    /// </summary>
    internal class BothHelper : NameSuffixHelper
    {
        public BothHelper(string Text)
            : base(Text)
        {
            _workingString = _text;
        }

        override protected string FinalString(string Input)
        {
            return Input;
        }
    }

    /// <summary>
    /// A static class of build NameSuffixHelpers
    /// </summary>
    internal static class NameSuffixHelperFactory
    {
        /// <summary>
        /// A static method to build the correct NameSuffixHelper object
        /// </summary>
        /// <param name="Text">The filename to pass to the NameSuffixHelper object</param>
        /// <param name="Behaviour">The type of NameSuffixHelper object to construct</param>
        /// <returns>A NameSuffixHelper</returns>
        public static NameSuffixHelper CreateNameSuffixHelper(string Text, NameSuffixBehaviour Behaviour)
        {
            switch (Behaviour)
            {
                case NameSuffixBehaviour.NameOnly:
                    return new NameHelper(Text);
                case NameSuffixBehaviour.SuffixOnly:
                    return new SuffixHelper(Text);
                case NameSuffixBehaviour.BothNameSuffix:
                    goto default;
                default:
                    return new BothHelper(Text);
            }
        }
    }

    /// <summary>
    /// A IFileRenamerStrategy to insert text into a filename.
    /// </summary>
    public class InsertTextStrategy : IFileRenamerStrategy
    {
        private int _position;
        private string _text;
        private bool _insert;
        private NameSuffixBehaviour _behaviour;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Position">Position to insert the text</param>
        /// <param name="Text">Text to insert into the filename</param>
        /// <param name="InsertOrOverwrite">Insert text (true) or overwrite? (false)</param>
        /// <param name="Behaviour">Affect the name only, suffix only, or both</param>
        public InsertTextStrategy(int Position, string Text, bool InsertOrOverwrite, NameSuffixBehaviour Behaviour)
        {
            _text = Text;
            _position = Position;
            _insert = InsertOrOverwrite;
            _behaviour = Behaviour;
        }

        /// <summary>
        /// Perform the renaming
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="Position">Position in the sequence (not used in this class)</param>
        /// <returns>The new filename</returns>
        public string RenameFile(FileMetaData FileName, int Position)
        {
            string newName;
            NameSuffixHelper nameSuffix = NameSuffixHelperFactory.CreateNameSuffixHelper(FileName.Name, _behaviour);

            if (_insert)
            {
                newName = nameSuffix.Insert(_position, _text);
            }
            else
            {
                newName = nameSuffix.Overwrite(_position, _text);
            }

            return newName;
        }
    }

    /// <summary>
    /// A IFileRenamerStrategy to remove characters from a filename
    /// </summary>
    public class RemoveCharactersStrategy : IFileRenamerStrategy
    {
        private int _fromPos;
        private int _toPos;
        private bool _fromLeft;
        private bool _toLeft;
        private NameSuffixBehaviour _behaviour;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="FromPos">The character position from which to start removing characters</param>
        /// <param name="FromLeft">Is the leftmost position calculated from the start(true) or the end?(false)</param>
        /// <param name="ToPos">The final character position we remove</param>
        /// <param name="ToLeft">Is the final position calculated from the start(true) or the end?(false)</param>
        /// <param name="Behaviour">Affect the name only, the suffix only, or both?</param>
        public RemoveCharactersStrategy(int FromPos, bool FromLeft, int ToPos, bool ToLeft, NameSuffixBehaviour Behaviour)
        {
            _fromPos = FromPos;
            _toPos = ToPos;
            _fromLeft = FromLeft;
            _toLeft = ToLeft;
            _behaviour = Behaviour;
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="FileName">The filename</param>
        /// <param name="Position">Position in the sequence (not used in this class)</param>
        /// <returns>The new filename.</returns>
        public string RenameFile(FileMetaData FileName, int Position)
        {
            NameSuffixHelper nameSuffix = NameSuffixHelperFactory.CreateNameSuffixHelper(FileName.Name, _behaviour);

            return nameSuffix.RemoveCharacters(_fromPos, _fromLeft, _toPos, _toLeft);
        }
    }

    /// <summary>
    /// The different numbering formats we can have
    /// </summary>
    public enum NumberingFormat
    {
        NoZeros,
        OneZero,
        TwoZero,
        ThreeZero,
        LowercaseLetters
    }

    /// <summary>
    /// The different ways in which a numbering format may be added to a filename.
    /// </summary>
    public enum NumberingTextFormat
    {
        OldNameTextNumber,
        NumberTextOldName,
        TextNumber,
        NumberText
    }

    /// <summary>
    /// An IFileRenamerStrategy to add a number to a filename.
    /// </summary>
    public class NumberingStrategy : IFileRenamerStrategy
    {
        private NameSuffixBehaviour _behaviour;
        private NumberingFormat _numberFormat;
        private NumberingTextFormat _textFormat;
        private string _start;
        private string _text;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="NumberFormat">Which number format to use</param>
        /// <param name="TextFormat">How to insert the numbering into a filename</param>
        /// <param name="Start">The starting number</param>
        /// <param name="Text">Any extra text to add</param>
        /// <param name="Behaviour">Affect the name only, the suffix only, or both</param>
        public NumberingStrategy(NumberingFormat NumberFormat, NumberingTextFormat TextFormat, string Start, string Text, NameSuffixBehaviour Behaviour)
        {
            _behaviour = Behaviour;
            _numberFormat = NumberFormat;
            _textFormat = TextFormat;
            _start = Start;
            _text = Text;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert a string into the equivalent integer.
        /// First converts to lowercase, then removes non-lowercase characters, before converting to a number.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private int StringToNumber(string Input)
        {
            // Convert to lowercase
            var lowercase = Input.ToLower();
            // Strip out non lowercase characters
            Regex rgx = new Regex("[^a-z]");
            var str = rgx.Replace(lowercase, "");
            int result = 0;
            // Convert to ASCII for value access
            byte[] asciiBytes = Encoding.ASCII.GetBytes(str);
            for (int i = 0; i < str.Length; i++)
            {
                // Subtract 96 since 'a' has value 97 in ASCII
                result = result * 26 + (asciiBytes[i] - 96);
                
            }
            return result;
        }

        private string NumberToString(int Num)
        {
            const string digits = "abcdefghiklmnopqrstuvwxyz";

            return "";
        }
    }


}