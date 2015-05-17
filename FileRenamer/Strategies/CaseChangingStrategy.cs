using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This strategy changes the case of a filename.  We currently have four strategies:
// lowercase, uppercase, camelcase, and sentencecase.
// Lowercase and uppercase are standard.  Camelcase capitalises the first letter of
// every word, and lowercases everything else.  Sentencecase capitalises the first letter 
// of the first word, otherwise it lowers the case.

namespace FileRenamer.Strategies
{
    /// <summary>
    /// Enum of the different case types
    /// </summary>
    public enum CaseTypes
    {
        Lowercase,
        Uppercase,
        Camelcase, // Capitalise every word
        Sentencecase // Capitalise start of a sentence
    }

    /// <summary>
    /// Strategy to change the case of a filename
    /// </summary>
    public class CaseChangingStrategy : IFileRenamerStrategy
    {
        private CaseTypes _caseType;

        /// <summary>
        /// The type of case changing to do
        /// </summary>
        public CaseTypes CaseType { get { return _caseType; } set { _caseType = value; OnChanged(EventArgs.Empty); } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CaseType">What case change to perform</param>
        public CaseChangingStrategy(CaseTypes CaseType)
        {
            _caseType = CaseType;
        }

        /// <summary>
        /// Compute the new filename
        /// </summary>
        /// <param name="FileName">IFileMetaData of the original file</param>
        /// <param name="Position">The position in the list of files</param>
        /// <param name="Helper">A helper to determine what part of the filename to modify</param>
        /// <returns></returns>
        public string RenameFile(IFileMetaData FileName, int Position, NameExtensionHelper Helper)
        {
            Helper.Text = FileName.Name;
            switch (_caseType)
            {
                case CaseTypes.Lowercase:
                    return Helper.Modify((s) => s.ToLower());
                case CaseTypes.Uppercase:
                    return Helper.Modify((s) => s.ToUpper());
                case CaseTypes.Camelcase:
                    return Helper.Modify((s) => toCamelCase(s));
                case CaseTypes.Sentencecase:
                    goto default;
                default:
                    return Helper.Modify((s) => toSentenceCase(s));
            }

        }

        // Change a string into camelcase
        private string toCamelCase(string Input)
        {
            // The idea is scan each character.  Any lowercase character after some
            // whitespace is made uppercase, any uppercase character seen not after
            // whitespace is made lowercase.
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
                else if (Char.IsUpper(Input, i))
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
                else if (Char.IsDigit(Input, i))
                {
                    seenWhitespace = false;
                    output.Append(Input[i]);
                }
                else
                {
                    output.Append(Input[i]);
                }
            }

            return output.ToString();
        }

        // Change a string into sentence case
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
                else if (Char.IsDigit(Input, i))
                {
                    seenPeriod = false;
                    output.Append(Input[i]);
                }
                else
                {
                    output.Append(Input[i]);
                }
            }

            return output.ToString();
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
}
