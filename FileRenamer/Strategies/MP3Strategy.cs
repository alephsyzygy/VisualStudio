using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This strategry renames a file using MP3 data.
// It is given a format string which it interprets to rename a file


// Format string info:
//Title: %t, Artist: %a,  Album: %b, Year: %y, Comment: %c, Genre: %g
//name: %n, extension: %x, full filename: %f
//position: %p
//Percent symbol: %%
//Other: %z => %z, %EOL => %

namespace FileRenamer.Strategies
{
    public class MP3Strategy : IFileRenamerStrategy
    {
        string _formatString;

        /// <summary>
        /// The format string the MP3 strategy uses to rename a file
        /// </summary>
        public string FormatString { get { return _formatString; } set { _formatString = value; OnChanged(EventArgs.Empty); } }

        public MP3Strategy(string FormatString)
        {
            _formatString = FormatString;
        }

        /// <summary>
        /// Rename a file using MP3 data
        /// </summary>
        /// <param name="FileName">The file meta data</param>
        /// <param name="Position">Position in the list</param>
        /// <param name="Helper">The helper to determine which part of the filename to rename</param>
        /// <returns>The new filename</returns>
        public string RenameFile(Model.IFileMetaData FileName, int Position, NameExtensionHelper Helper)
        {
            // The idea: scan through the formatstring, character by character until a % is found,
            // then switch on the next character.
            Helper.Text = FileName.Name;
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < _formatString.Count(); i++ )
            {
                if (_formatString[i]!='%')
                {
                    output.Append(_formatString[i]);
                } 
                else
                {
                    if (i == _formatString.Count() - 1)
                    {
                        // we are at the end of the string
                        output.Append('%');
                    }
                    else
                    {
                        i++; // advance to next character
                        char nextChar = _formatString[i];
                        switch (nextChar)
                        {
                            case 'a': // artist
                                if (FileName.ID3Tag != null)
                                    output.Append(FileName.ID3Tag.Artist);
                                break;
                            case 'b': // album
                                if (FileName.ID3Tag != null)
                                    output.Append(FileName.ID3Tag.Album);
                                break;
                            case 'c': // comment
                                if (FileName.ID3Tag != null)
                                    output.Append(FileName.ID3Tag.Comment);
                                break;
                            case 'f':  // full filename
                                output.Append(FileName.Name);
                                break;
                            case 'g': // genre
                                if (FileName.ID3Tag != null)
                                    output.Append(FileName.ID3Tag.Genre);
                                break;
                            case 'n': // first part of filename
                                output.Append(Helper.Name);
                                break;
                            case 'p': // position in list
                                output.Append(Position);
                                break;
                            case 't': // title
                                if (FileName.ID3Tag != null)
                                    output.Append(FileName.ID3Tag.Title);
                                break;
                            case 'x': // extension of file
                                output.Append(Helper.Extension);
                                break;
                            case 'y': // year
                                if (FileName.ID3Tag != null)
                                    output.Append(FileName.ID3Tag.Year);
                                break;
                            case '%': // escaped %
                                output.Append('%');
                                break;
                            default:
                                output.Append('%');
                                output.Append(nextChar);
                                break;
                        }
                    }


                }
            }
            return Helper.Modify(oldname => output.ToString());
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
