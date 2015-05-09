using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    /// <summary>
    /// A IFileRenamerStrategy to remove characters from a filename
    /// </summary>
    public class RemoveCharactersStrategy : IFileRenamerStrategy
    {
        private int _fromPos;
        private int _toPos;
        private bool _fromLeft;
        private bool _toLeft;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="FromPos">The character position from which to start removing characters</param>
        /// <param name="FromLeft">Is the leftmost position calculated from the start(true) or the end?(false)</param>
        /// <param name="ToPos">The final character position we remove</param>
        /// <param name="ToLeft">Is the final position calculated from the start(true) or the end?(false)</param>
        /// <param name="Behaviour">Affect the name only, the suffix only, or both?</param>
        public RemoveCharactersStrategy(int FromPos, bool FromLeft, int ToPos, bool ToLeft)
        {
            _fromPos = FromPos;
            _toPos = ToPos;
            _fromLeft = FromLeft;
            _toLeft = ToLeft;
        }

        public int FromPosition { get { return _fromPos; } set { _fromPos = value; OnChanged(EventArgs.Empty); } }
        public int ToPosition { get { return _toPos; } set { _toPos = value; OnChanged(EventArgs.Empty); } }
        public bool FromLeft { get { return _fromLeft; } set { _fromLeft = value; OnChanged(EventArgs.Empty); } }
        public bool ToLeft { get { return _toLeft; } set { _toLeft = value; OnChanged(EventArgs.Empty); } }


        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="FileName">The filename</param>
        /// <param name="Position">Position in the sequence (not used in this class)</param>
        /// <returns>The new filename.</returns>
        public string RenameFile(FileMetaData FileName, int Position, NameSuffixHelper Helper)
        {
            Helper.Text = FileName.Name;
            return Helper.RemoveCharacters(_fromPos, _fromLeft, _toPos, _toLeft);
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
