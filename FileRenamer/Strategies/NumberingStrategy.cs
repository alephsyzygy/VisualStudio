using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
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
        private NumberingFormat _numberFormat;
        private NumberingTextFormat _textFormat;
        private int _start;
        private string _text;
        private string _startString;

        public string Start { get { return _startString; } set { _startString = value; FixStartString();  OnChanged(EventArgs.Empty); } }
        public string Text { get { return _text; } set { _text = value; OnChanged(EventArgs.Empty); } }
        public NumberingFormat NumberFormat { get { return _numberFormat; } set { _numberFormat = value; OnChanged(EventArgs.Empty); } }
        public NumberingTextFormat TextFormat { get { return _textFormat; } set { _textFormat = value; OnChanged(EventArgs.Empty); } }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="NumberFormat">Which number format to use</param>
        /// <param name="TextFormat">How to insert the numbering into a filename</param>
        /// <param name="Start">The starting number</param>
        /// <param name="Text">Any extra text to add</param>
        /// <param name="Behaviour">Affect the name only, the Extension only, or both</param>
        public NumberingStrategy(NumberingFormat NumberFormat, NumberingTextFormat TextFormat, string Start, string Text)
        {
            _numberFormat = NumberFormat;
            _textFormat = TextFormat;
            _text = Text;
            _startString = Start;
            FixStartString();
        }

        private void FixStartString()
        {
            // First try to parse start as an int.
            if (!Int32.TryParse(_startString, out _start))
            {
                // if that fails, see if we are using the lowercase numbering format
                if (_numberFormat == NumberingFormat.LowercaseLetters)
                {
                    _start = StringNumberConversions.StringToNumber(_startString);
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

        public string RenameFile(FileMetaData FileName, int Position, NameExtensionHelper Helper)
        {
            Helper.Text = FileName.Name;
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
                    numberString = InsertZeros(num, 2);
                    break;
                case NumberingFormat.ThreeZeros:
                    numberString = InsertZeros(num, 3);
                    break;
                case NumberingFormat.NoZeros:
                    goto default;
                default:
                    numberString = num.ToString();
                    break;
            }

            // This function determines the text format, we will pass it to modify in the NameExtensionHelper
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

            return Helper.Modify(_textFormatFn);
        }

        #region Events

        public event EventHandler<EventArgs> StrategyChanged;

        protected void OnChanged(EventArgs e)
        {
            if (StrategyChanged != null)
                StrategyChanged(this, e);
        }

        #endregion

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
}
