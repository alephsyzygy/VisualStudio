using System;
using System.Text;
using System.Text.RegularExpressions;
namespace FileRenamer.Old
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
        /// Constructor.  Cannot be constructed by otherclasses, instead use the static method CreateNameSuffixHelper.
        /// </summary>
        /// <param name="Text">The filename</param>
        /// <param name="Behaviour">What behaviour to use for inserting and overwriting</param>
        protected NameSuffixHelper(string Text)
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

        public string Insert (int Position, bool FromLeft, string Text)
        {
            if (!FromLeft)
            {
                Position = _workingString.Length - Position;
            }
            return Insert(Position, Text);
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

        public string Overwrite(int Position, bool FromLeft, string Text)
        {
            if (!FromLeft)
            {
                Position = _workingString.Length - Position;
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
            return InputText.Insert(Math.Max(0,Math.Min(Position, InputText.Length)), InsertText);
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

        /// <summary>
        /// Run a function on the part we want to modify, then reassemble into our filename
        /// </summary>
        /// <param name="ModifyFn">Function which takes a string and returns a string</param>
        /// <returns>The new filename</returns>
        public string Modify(Func<string,string> ModifyFn)
        {
            string newStr = ModifyFn(_workingString);

            return FinalString(newStr);
        }

        public string RegexReplace(string Find, string Replace, bool IsCaseSensitive)
        {
            RegexOptions options = RegexOptions.None;
            if (IsCaseSensitive)
            {
                options = RegexOptions.IgnoreCase;
            }
            try
            {
                Regex regex = new Regex(Find, options);

                return FinalString(regex.Replace(_workingString, Replace));
            }
            catch (System.ArgumentException)
            {
                return FinalString(_workingString);
            }
        }

        public string Replace(string Find, string Replace, bool IsCaseSensitive)
        {
            if (!IsCaseSensitive)
            {
                return FinalString(_workingString.Replace(Find, Replace));
            }
            else
            {
                return FinalString(CaseInsensitiveReplace(_workingString, Find, Replace));
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
    /// A NameSuffixHelper which only modifies the name of a filename.
    /// </summary>
    internal class NameHelper : NameSuffixHelper 
    { 
        protected internal NameHelper(string Text) : base(Text)
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
        protected internal SuffixHelper(string Text)
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
        protected internal BothHelper(string Text)
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
    /// A IFileRenamerStrategy to insert text into a filename.
    /// </summary>
    public class InsertTextStrategy : IFileRenamerStrategy
    {
        private int _position;
        private string _text;
        private bool _insert;
        private bool _fromLeft;
        private NameSuffixBehaviour _behaviour;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Position">Position to insert the text</param>
        /// <param name="Text">Text to insert into the filename</param>
        /// <param name="InsertOrOverwrite">Insert text (true) or overwrite? (false)</param>
        /// <param name="Behaviour">Affect the name only, suffix only, or both</param>
        public InsertTextStrategy(int Position, string Text, bool FromLeft, bool InsertOrOverwrite, NameSuffixBehaviour Behaviour)
        {
            _text = Text;
            _position = Position;
            _insert = InsertOrOverwrite;
            _behaviour = Behaviour;
            _fromLeft = FromLeft;
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
            NameSuffixHelper nameSuffix = NameSuffixHelper.CreateNameSuffixHelper(FileName.Name, _behaviour);

            if (_insert)
            {
                newName = nameSuffix.Insert(_position, _fromLeft, _text);
            }
            else
            {
                newName = nameSuffix.Overwrite(_position,_fromLeft, _text);
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
            NameSuffixHelper nameSuffix = NameSuffixHelper.CreateNameSuffixHelper(FileName.Name, _behaviour);
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
        TwoZeros,
        ThreeZeros,
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
        private int _start;
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
            _text = Text;

            // First try to parse start as an int.
            if (!Int32.TryParse(Start, out _start))
            {
                // if that fails, see if we are using the lowercase numbering format
                if (_numberFormat == NumberingFormat.LowercaseLetters)
                {
                    _start = StringNumberConversions.StringToNumber(Start);
                }
                else
                {
                    // otherwise default to starting at 1
                    _start = 1;
                }
            }
        }

        /// <summary>
        /// Internal method to print numbers with the given number of zeros out the front.
        /// e.g. if MaxZeros is 1, then 1 -> 01, 2 -> 02, 10 -> 10, 11 -> 11 and so on.
        /// </summary>
        /// <param name="Number">The number to format</param>
        /// <param name="MaxZeros">The maximum number of zeros to append to the start</param>
        /// <returns>The formated number</returns>
        private string InsertZeros(int Number, int MaxZeros)
        {
            if (Number <= 0)
            {
                return Number.ToString();
            }

            // find the length of the input number in base 10
            int length = (int)Math.Truncate(Math.Log10(Number));
            // find how many zeros to append
            int numZeros = MaxZeros - length;
            string zeros;

            if (numZeros > 0)
            {
                // create the string of zeros
                zeros = new string('0', numZeros);
            } 
            else
            {
                zeros = "";
            }

            return zeros + Number.ToString();
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            NameSuffixHelper nameSuffix = NameSuffixHelper.CreateNameSuffixHelper(FileName.Name, _behaviour);
            string numberString;
            int num = Position + _start;

            // Format the position number into the correct format
            switch (_numberFormat)
            {
                case NumberingFormat.LowercaseLetters:
                    numberString = StringNumberConversions.NumberToString(num);
                    break;
                case NumberingFormat.OneZero:
                    numberString = InsertZeros(num, 1);
                    break;
                case NumberingFormat.TwoZeros:
                    numberString =  InsertZeros(num, 2);
                    break;
                case NumberingFormat.ThreeZeros:
                    numberString =  InsertZeros(num, 3);
                    break;
                case NumberingFormat.NoZeros:
                    goto default;
                default:
                    numberString = num.ToString();
                    break;
            }

            // This function determines the text format, we will pass it to modify in the NameSuffixHelper
            Func<String, String> _textFormatFn;

            switch (_textFormat)
            {
                case NumberingTextFormat.NumberText:
                    _textFormatFn = (OldName) => numberString + _text;
                    break;
                case NumberingTextFormat.NumberTextOldName:
                    _textFormatFn = (OldName) => numberString + _text + OldName;
                    break;
                case NumberingTextFormat.OldNameTextNumber:
                    _textFormatFn = (OldName) => OldName + _text + numberString;
                    break;
                case NumberingTextFormat.TextNumber:
                    goto default;
                default:
                    _textFormatFn = (OldName) => _text + numberString;
                    break;
            }

            return nameSuffix.Modify(_textFormatFn);
        }
        
    }

    /// <summary>
    /// A static class to help convert between strings and numbers, using the same numbering system as spreadsheet columns
    /// </summary>
    public static class StringNumberConversions
    {
        /// <summary>
        /// Convert a string into the equivalent integer.
        /// First converts to lowercase, then removes non-lowercase characters, before converting to a number.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static int StringToNumber(string Input)
        {
            const int ASCIIvaluefora = 97;
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
                result = result * 26 + (asciiBytes[i] - ASCIIvaluefora + 1);

            }
            return result;
        }

        /// <summary>
        /// Convert a number into the equivalent string.
        /// Note: this is not equivalent to any number base, in particular it is NOT base 26
        /// There is no zero in this system: a stands for 1, but aa stands for 27
        /// In a based system 
        /// </summary>
        /// <param name="Num">The number to convert</param>
        /// <returns>The corresponding string</returns>
        public static string NumberToString(int Num)
        {
            const string DIGITS = "abcdefghijklmnopqrstuvwxyz";
            const int BASE = 26;

            string output = "";

            // Since this system is not a standard base we have to 'cut away' some numbers
            // to find the length of the resulting number.
            // For example, aa should correspond to 27.  We see that it is bigger 26, yet
            // smaller than 26^2 = 676.  Hence it has length 2.
            int y = 1;
            int x = Num;
            int count = 0;
            while (y <= x)
            {
                x = x - y;
                y = y * BASE;
                count++;
            }


            // Standard base conversion technique
            int rem;
            for (int i = 0; i < count; i++)
            {
                x = Math.DivRem(x, BASE, out rem);
                output = output + DIGITS[rem];

            }

            // Now reverse the string
            char[] charArray = output.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    public enum CaseTypes
    {
        Lowercase,
        Uppercase,
        Camelcase,
        Sentencecase
    }

    public class CaseChangerStrategy : IFileRenamerStrategy
    {
        private CaseTypes _caseType;
        private NameSuffixBehaviour _behaviour;

        public CaseChangerStrategy(CaseTypes CaseType, NameSuffixBehaviour Behaviour)
        {
            _caseType = CaseType;
            _behaviour = Behaviour;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            NameSuffixHelper nameSuffix = NameSuffixHelper.CreateNameSuffixHelper(FileName.Name, _behaviour);

            switch (_caseType)
            {
                case CaseTypes.Lowercase:
                    return nameSuffix.Modify((s) => s.ToLower());
                case CaseTypes.Uppercase:
                    return nameSuffix.Modify((s) => s.ToUpper());
                case CaseTypes.Camelcase:
                    return nameSuffix.Modify((s) => toCamelCase(s));
                case CaseTypes.Sentencecase:
                    goto default;
                default:
                    return nameSuffix.Modify((s) => toSentenceCase(s));
            }

        }

        private string toCamelCase(string Input)
        {
            bool seenWhitespace = true;
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < Input.Length; i++)
            {
                if (Char.IsWhiteSpace(Input, i))
                {
                    seenWhitespace = true;
                    output.Append(Input[i]);
                }
                else if (Char.IsLower(Input, i))
                {
                    if (seenWhitespace)
                    {
                        seenWhitespace = false;
                        output.Append(Input[i].ToString().ToUpper());
                    }
                    else
                    {
                        output.Append(Input[i]);
                    }
                }
                else if (Char.IsUpper(Input,i))
                {
                    if (seenWhitespace)
                    {
                        seenWhitespace = false;
                        output.Append(Input[i]);
                    }
                    else
                    {
                        output.Append(Input[i].ToString().ToLower());
                    }
                }
                else
                {
                    output.Append(Input[i]);
                }
            }

            return output.ToString();
        }

        private string toSentenceCase(string Input)
        {
            bool seenPeriod = true;
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < Input.Length; i++)
            {
                if (Input[i] == '.')
                {
                    seenPeriod = true;
                    output.Append(Input[i]);
                }
                else if (Char.IsLower(Input, i))
                {
                    if (seenPeriod)
                    {
                        seenPeriod = false;
                        output.Append(Input[i].ToString().ToUpper());
                    }
                    else
                    {
                        output.Append(Input[i]);
                    }
                }
                else if (Char.IsUpper(Input, i))
                {
                    if (seenPeriod)
                    {
                        seenPeriod = false;
                        output.Append(Input[i]);
                    }
                    else
                    {
                        output.Append(Input[i].ToString().ToLower());
                    }
                }
                else
                {
                    output.Append(Input[i]);
                }
            }

            return output.ToString();
        }
    }

    public enum DateTimeType
    {
        Current,
        Created,
        Modified,
        PhotoTaken
    }

    public class DateInserterStrategy : IFileRenamerStrategy
    {
        private DateTimeType _dateType;
        private int _position;
        private bool _fromLeft;
        private string _dateFormat;
        private NameSuffixBehaviour _behaviour;

        public DateInserterStrategy(DateTimeType DateType, int Position, bool FromLeft, string DateFormat, NameSuffixBehaviour Behaviour)
        {
            _dateType = DateType;
            _position = Position;
            _fromLeft = FromLeft;
            _dateFormat = DateFormat;
            _behaviour = Behaviour;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            NameSuffixHelper nameSuffix = NameSuffixHelper.CreateNameSuffixHelper(FileName.Name, _behaviour);
            DateTime date;
            switch (_dateType)
            {
                case DateTimeType.Current:
                    date = DateTime.Now;
                    break;
                case DateTimeType.Created:
                    date = FileName.Created;
                    break;
                case DateTimeType.Modified:
                    date = FileName.Modified;
                    break;
                case DateTimeType.PhotoTaken:
                    goto default;
                default:
                    date = DateTime.Now;
                    break;
            }
            string dateString;
            try
            {
                dateString = String.Format("{0:" + _dateFormat + "}", date);
            } catch (FormatException)
            {
                dateString = "";
            }
            return nameSuffix.Insert(_position, _fromLeft, dateString);
        }
    }

    public class SearchReplaceStrategy : IFileRenamerStrategy
    {
        private string _searchRegex;
        private string _replaceRegex;
        private bool _useRegex;
        private bool _caseSensitive;
        private NameSuffixBehaviour _behaviour;

        public SearchReplaceStrategy(string Find, string Replace, bool useRegex, bool caseSensitive, NameSuffixBehaviour Behaviour)
        {
            _searchRegex = Find;
            _replaceRegex = Replace;
            _useRegex = useRegex;
            _caseSensitive = caseSensitive;
            _behaviour = Behaviour;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            NameSuffixHelper nameSuffix = NameSuffixHelper.CreateNameSuffixHelper(FileName.Name, _behaviour);

            if (_useRegex)
            {
                return nameSuffix.RegexReplace(_searchRegex, _replaceRegex, _caseSensitive);
            }
            else
            {
                return nameSuffix.Replace(_searchRegex, _replaceRegex, _caseSensitive);
            }

        }
    }


}