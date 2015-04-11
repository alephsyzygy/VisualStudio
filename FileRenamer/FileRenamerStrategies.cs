using System;
using System.Text;
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
    /// </summary>
    public class NameSuffixHelper
    {
        private string _text;
        private bool _hasSeparator;
        private string _name;
        private string _suffix;
        private int _suffixPos;
        private NameSuffixBehaviour _behaviour;

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
        public NameSuffixHelper(string Text, NameSuffixBehaviour Behaviour)
        {
            _behaviour = Behaviour;
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
            string newName;
            string newSuffix;

            switch (_behaviour)
            {
                case NameSuffixBehaviour.NameOnly:
                    newName = InsertOrAppend(_name, Position, Text);
                    newSuffix = _suffix;
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.SuffixOnly:
                    newName = _name;
                    newSuffix = InsertOrAppend(_suffix, Position, Text);
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.BothNameSuffix:
                    goto default;
                default:
                    return InsertOrAppend(_text, Position, Text);
            }
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
            string newName;
            string newSuffix;

            switch (_behaviour)
            {
                case NameSuffixBehaviour.NameOnly:
                    newName = OverwriteOrAppend(_name, Position, Text);
                    newSuffix = _suffix;
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.SuffixOnly:
                    newName = _name;
                    newSuffix = OverwriteOrAppend(_suffix, Position, Text);
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.BothNameSuffix:
                    goto default;
                default:
                    return OverwriteOrAppend(_text, Position, Text);
            }
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
    }

    public class InsertTextStrategy : IFileRenamerStrategy
    {
        private int _position;
        private string _text;
        private bool _insert;
        private NameSuffixBehaviour _behaviour;

        public InsertTextStrategy(int Position, string Text, bool InsertOrOverwrite, NameSuffixBehaviour Behaviour)
        {
            _text = Text;
            _position = Position;
            _insert = InsertOrOverwrite;
            _behaviour = Behaviour;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            string newName;
            NameSuffixHelper nameSuffix = new NameSuffixHelper(FileName.Name, _behaviour);

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

    public class RemoveCharactersStrategy : IFileRenamerStrategy
    {
        private int _fromPos;
        private int _toPos;
        private bool _fromLeft;
        private bool _toLeft;

        public RemoveCharactersStrategy(int FromPos, bool FromLeft, int ToPos, bool ToLeft)
        {
            _fromPos = FromPos;
            _toPos = ToPos;
            _fromLeft = FromLeft;
            _toLeft = ToLeft;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            string oldName = FileName.Name;
            string newName;

            int leftPos;
            int rightPos;

            // Calculate leftmost character to delete
            if (_fromLeft)
            {
                leftPos = _fromPos;
            }
            else
            {
                leftPos = oldName.Length - _fromPos;
            }

            // Calculate rightmost character to delete
            if (_toLeft)
            {
                rightPos = _toPos;
            }
            else
            {
                rightPos = oldName.Length - _toPos;
            }

            // Clamp left position
            if (leftPos < 0)
            {
                leftPos = 0;
            }

            // Clamp right position
            if (rightPos > oldName.Length)
            {
                rightPos = oldName.Length;
            }

            // Perform deletion
            if (rightPos - leftPos >= 0)
            {
                newName = oldName.Remove(leftPos, rightPos - leftPos);
            }
            else
            {
                newName = oldName;
            }

            return newName;

        }
    }


}