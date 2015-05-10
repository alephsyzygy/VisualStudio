using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    public class SearchReplaceStrategy : IFileRenamerStrategy
    {
        private string _searchRegex;
        private string _replaceRegex;
        private bool _useRegex;
        private bool _caseSensitive;

        public string SearchRegex { get { return _searchRegex; } set { _searchRegex = value; OnChanged(EventArgs.Empty); } }
        public string ReplaceRegex { get { return _replaceRegex; } set { _replaceRegex = value; OnChanged(EventArgs.Empty); } }
        public bool UseRegex { get { return _useRegex; } set { _useRegex = value; OnChanged(EventArgs.Empty); } }
        public bool CaseSensitive { get { return _caseSensitive; } set { _caseSensitive = value; OnChanged(EventArgs.Empty); } }

        public SearchReplaceStrategy(string Find, string Replace, bool useRegex, bool caseSensitive)
        {
            _searchRegex = Find;
            _replaceRegex = Replace;
            _useRegex = useRegex;
            _caseSensitive = caseSensitive;
        }

        public string RenameFile(FileMetaData FileName, int Position, NameExtensionHelper Helper)
        {
            Helper.Text = FileName.Name;

            if (_useRegex)
            {
                return Helper.RegexReplace(_searchRegex, _replaceRegex, _caseSensitive);
            }
            else
            {
                return Helper.Replace(_searchRegex, _replaceRegex, _caseSensitive);
            }

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
