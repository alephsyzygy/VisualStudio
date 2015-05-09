using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    public enum CaseTypes
    {
        Lowercase,
        Uppercase,
        Camelcase,
        Sentencecase
    }

    public class CaseChangingStrategy : IFileRenamerStrategy
    {
        private CaseTypes _caseType;

        public CaseTypes CaseType { get { return _caseType; } set { _caseType = value; OnChanged(EventArgs.Empty); } }

        public CaseChangingStrategy(CaseTypes CaseType)
        {
            _caseType = CaseType;
        }

        public string RenameFile(FileMetaData FileName, int Position, NameSuffixHelper Helper)
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
