using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    /// <summary>
    /// Different behaviours for dealing with Extensiones:
    ///   NameOnly: only modify the name
    ///   ExtensionOnly: only modify the Extension
    ///   BothNameExtension: modify anything in the filename, including the separator
    /// The separator is the last period "." found in a file name, if it has one.
    /// The Extension is anything after the separator and the name is anything before.
    /// If a file name does not have a separator then the name is the whole file name, and the Extension is empty
    /// </summary>
    public enum NameExtensionBehaviour
    {
        NameOnly,
        ExtensionOnly,
        BothNameExtension
    };

    /// <summary>
    /// A class to help deal with names and Extensiones.
    /// This class is a template, the different behaviours will be given by subclasses
    /// </summary>
    abstract public class NameExtensionHelper
    {
        protected string _text;
        protected bool _hasSeparator;
        protected string _name;
        protected string _Extension;
        protected int _ExtensionPos;

        /// <summary>
        /// This method will be overriden by subclasses in order to compute the final string.
        /// </summary>
        /// <param name="Input">The name with which we need modify</param>
        /// <returns>String with name, Extension, and/or separator added to it, depending on which subclass is used</returns>
        abstract protected string FinalString(string Input);

        abstract protected string WorkingString();

        /// <summary>
        /// Where in the original text the Extension starts
        /// </summary>
        public int ExtensionPos
        {
            get { return _ExtensionPos; }
        }

        /// <summary>
        /// Does the filename have a separator
        /// </summary>
        public bool HasSeparator
        {
            get { return _hasSeparator; }
        }

        /// <summary>
        /// The Extension of the file name
        /// </summary>
        public string Extension
        {
            get { return _Extension; }
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
            set
            {
                _text = value;
                _hasSeparator = _text.Contains(".");
                int finalDotPos = _text.LastIndexOf(".");
                _Extension = _text.Substring(finalDotPos + 1);
                if (finalDotPos >= 0)
                {
                    _name = _text.Substring(0, finalDotPos);
                    _ExtensionPos = finalDotPos;
                }
                else
                {
                    _name = _text;
                    _ExtensionPos = _text.Length;
                    _Extension = "";
                }
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
        /// Constructor.  Cannot be constructed by otherclasses, instead use the static method CreateNameExtensionHelper.
        /// </summary>
        /// <param name="Text">The filename</param>
        /// <param name="Behaviour">What behaviour to use for inserting and overwriting</param>
        protected NameExtensionHelper()
        {
            this.Text = "";
            //_text = Text;
            //_hasSeparator = _text.Contains(".");
            //int finalDotPos = _text.LastIndexOf(".");
            //_Extension = _text.Substring(finalDotPos + 1);
            //if (finalDotPos >= 0)
            //{
            //    _name = _text.Substring(0, finalDotPos);
            //    _ExtensionPos = finalDotPos;
            //}
            //else
            //{
            //    _name = _text;
            //    _ExtensionPos = _text.Length;
            //}
        }

        public static NameExtensionHelper CreateNameExtensionHelper(NameExtensionBehaviour Behaviour)
        {
            switch (Behaviour)
            {
                case NameExtensionBehaviour.NameOnly:
                    return new NameHelper();
                case NameExtensionBehaviour.ExtensionOnly:
                    return new ExtensionHelper();
                case NameExtensionBehaviour.BothNameExtension:
                    goto default;
                default:
                    return new BothHelper();
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
            return FinalString(InsertOrAppend(WorkingString(), Position, Text));
        }

        public string Insert(int Position, bool FromLeft, string Text)
        {
            if (!FromLeft)
            {
                Position = WorkingString().Length - Position;
            }
            return Insert(Position, Text);
        }

        /// <summary>
        /// Overwrite parts of the filename with the given text, at the given position.
        /// The name/Extension behaviour is determined by the constructor.
        /// If the text to overwrite overruns the original text then the length is extended.
        /// If the position is greater than the length of the text then it appends.
        /// </summary>
        /// <param name="Position">Position to overwrite</param>
        /// <param name="Text">Text to overwrite</param>
        /// <returns>New filename</returns>
        public string Overwrite(int Position, string Text)
        {
            return FinalString(OverwriteOrAppend(WorkingString(), Position, Text));
        }

        public string Overwrite(int Position, bool FromLeft, string Text)
        {
            if (!FromLeft)
            {
                Position = WorkingString().Length - Position;
            }
            return Overwrite(Position, Text);
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
            return InputText.Insert(Math.Max(0, Math.Min(Position, InputText.Length)), InsertText);
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
            // Clamp Position to a sensible value
            Position = Math.Max(0, Math.Min(Position, InputText.Length));
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

            currentLength = WorkingString().Length;

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
                newName = WorkingString().Remove(leftPos, rightPos - leftPos);
            }
            else
            {
                newName = WorkingString();
            }

            // Get final string via subclasses
            return FinalString(newName);
        }

        /// <summary>
        /// Run a function on the part we want to modify, then reassemble into our filename
        /// </summary>
        /// <param name="ModifyFn">Function which takes a string and returns a string</param>
        /// <returns>The new filename</returns>
        public string Modify(Func<string, string> ModifyFn)
        {
            string newStr = ModifyFn(WorkingString());

            return FinalString(newStr);
        }

        public string RegexReplace(string Find, string Replace, bool IsCaseSensitive)
        {
            RegexOptions options = RegexOptions.None;
            if (!IsCaseSensitive)
            {
                options = RegexOptions.IgnoreCase;
            }
            try
            {
                Regex regex = new Regex(Find, options);

                return FinalString(regex.Replace(WorkingString(), Replace));
            }
            catch (System.ArgumentException)
            {
                return FinalString(WorkingString());
            }
        }

        public string Replace(string Find, string Replace, bool IsCaseSensitive)
        {
            if (Find != "")
            {
                if (IsCaseSensitive)
                {
                    return FinalString(WorkingString().Replace(Find, Replace));
                }
                else
                {
                    return FinalString(CaseInsensitiveReplace(WorkingString(), Find, Replace));
                }
            }
            else
            {
                return FinalString(WorkingString());
            }
        }

        private string CaseInsensitiveReplace(string Input, string Find, string Replace)
        {
            int position = 0;   // Where we have checked up to
            int index = 0; // Index of next match

            if (Input == "")
            {
                return Input;
            }

            if (Find == null || Find == "")
            {
                return Input;
            }

            StringBuilder Output = new StringBuilder();
            while (true)
            {
                index = Input.IndexOf(Find, position, StringComparison.CurrentCultureIgnoreCase);
                if (index < 0)
                {
                    break;
                }
                Output.Append(Input, position, index - position);
                Output.Append(Replace);
                position = index + Find.Length;
            }
            Output.Append(Input, position, Input.Length - position);
            return Output.ToString();
        }
    }


    /// <summary>
    /// A NameExtensionHelper which only modifies the name of a filename.
    /// </summary>
    internal class NameHelper : NameExtensionHelper
    {
        override protected string WorkingString()
        {
            return _name;
        }

        override protected string FinalString(string Input)
        {
            return Input + SeparatorString + _Extension;
        }
    }

    /// <summary>
    /// A NameExtensionHelper which only modifies the Extension of a filename.
    /// </summary>
    internal class ExtensionHelper : NameExtensionHelper
    {
        override protected string WorkingString()
        {
            return _Extension;
        }

        override protected string FinalString(string Input)
        {
            return _name + SeparatorString + Input;
        }
    }

    /// <summary>
    /// A NameExtensionHelper which modifies any part of a filename.
    /// </summary>
    internal class BothHelper : NameExtensionHelper
    {
        override protected string WorkingString()
        {
            return _text;
        }

        override protected string FinalString(string Input)
        {
            return Input;
        }
    }
}
