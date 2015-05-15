using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    /// <summary>
    /// A IFileRenamerStrategy to insert text into a filename.
    /// </summary>
    public class InsertTextStrategy : IFileRenamerStrategy
    {
        int _position;
        string _text;
        bool _fromLeft;
        bool _insertOrOverwrite;

        public int Position { get { return _position; } set { _position = value; OnChanged(EventArgs.Empty); } }
        public string Text { get { return _text; } set { _text = value; OnChanged(EventArgs.Empty); } }
        public bool FromLeft { get { return _fromLeft; } set { _fromLeft = value; OnChanged(EventArgs.Empty); } }
        public bool InsertOrOverwrite { get { return _insertOrOverwrite; } set { _insertOrOverwrite = value; OnChanged(EventArgs.Empty); } }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="Position">Position to insert the text</param>
        /// <param name="Text">Text to insert into the filename</param>
        /// <param name="InsertOrOverwrite">Insert text (true) or overwrite? (false)</param>
        /// <param name="Behaviour">Affect the name only, Extension only, or both</param>
        public InsertTextStrategy(int Position, string Text, bool FromLeft, bool InsertOrOverwrite)
        {
            this.Text = Text;
            this.Position = Position;
            this.FromLeft = FromLeft;
            this.InsertOrOverwrite = InsertOrOverwrite;
        }

        /// <summary>
        /// Perform the renaming
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="Position">Position in the sequence (not used in this class)</param>
        /// <returns>The new filename</returns>
        public string RenameFile(IFileMetaData FileName, int Position, NameExtensionHelper Helper)
        {
            string newName;
            Helper.Text = FileName.Name;
            //NameExtensionHelper nameExtension = NameExtensionHelper.CreateNameExtensionHelper(FileName.Name, _behaviour);

            if (InsertOrOverwrite)
            {
                newName = Helper.Insert(_position, FromLeft, Text);
            }
            else
            {
                newName = Helper.Overwrite(_position, FromLeft, Text);
            }

            return newName;
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
