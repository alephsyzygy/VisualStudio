using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Strategies;
using FileRenamer.IO;

namespace FileRenamer.Model
{
    class RenamerModel
    {
        #region Fields

        ObservableCollection<FileMetaData> _fileMetaData;
        IFileRenamerStrategy _renameStrategy;
        NameExtensionHelper _helper;

        #endregion

        #region Constructor

        public RenamerModel(List<FileMetaData> FileNames)
        {
            _fileMetaData = new ObservableCollection<FileMetaData>(FileNames);
            _renameStrategy = new IdentityStrategy();
            Files = new ObservableCollection<FileModel>();
            foreach (var file in _fileMetaData)
            {
                FileModel fileModel = new FileModel(file.Name);
                Files.Add(fileModel);
            }
            UpdateFiles();
        }

        #endregion

        #region Properties

        public ObservableCollection<FileModel> Files { get; private set; }
        
        public IFileRenamerStrategy RenameStrategy
        {
            get { return _renameStrategy; }
            set
            {
                _renameStrategy.StrategyChanged -= OnStrategyChanged;
                _renameStrategy = value;
                _renameStrategy.StrategyChanged += OnStrategyChanged;
                UpdateFiles();
                if (StrategyChanged != null)
                    StrategyChanged(this, EventArgs.Empty);
            }
        }

        public NameExtensionHelper Helper
        {
            get { return _helper; }
            set
            {
                _helper = value;
                UpdateFiles();
                if (StrategyChanged != null)
                    StrategyChanged(this, EventArgs.Empty);
            }
        }

        public bool Clashes { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Move an element in the list to another position
        /// </summary>
        /// <param name="from">index of element to move</param>
        /// <param name="to">index it moves to.  If this is negative then move to end</param>
        public void Move(int from, int to)
        {
            if (to < 0)
            {
                to = _fileMetaData.Count - 1;  // Move to last location
            }
            _fileMetaData.Move(from, to);
            Files.Move(from, to);
            UpdateFiles();
        }

        #endregion

        #region Events

        public event EventHandler<EventArgs> StrategyChanged;
        public event EventHandler<EventArgs> FilesChanged;

        #endregion

        #region Private Helpers

        void OnStrategyChanged(object sender, EventArgs e)
        {
            UpdateFiles();
        }

        private void UpdateFiles()
        {
            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].NewFileName = _renameStrategy.RenameFile(_fileMetaData[i], i, _helper);
                Files[i].Clashes = false;
            }
            CheckForClashes();
        }

        private void CheckForClashes()
        {
            // Distinct() uses a hashtable internally so is O(n)
            this.Clashes = Files.Count != Files.Select<FileModel,String>(file => file.NewFileName).Distinct().Count();

            if (this.Clashes)
            {
                // Mark all the elements that clash
                var duplicates = Files.GroupBy<FileModel, String>(file => file.NewFileName)
                    .Where(group => group.Count() > 1)
                    .SelectMany(group => group);
                //var duplicates = _fileMetaData.Zip(_newFileNames, (a, b) => Tuple.Create(a, b)).GroupBy(x => x.Item2)
                //        .Where(group => group.Count() > 1)
                //        .SelectMany(group => group);
                foreach (var files in duplicates)
                {
                    files.Clashes = true;
                }
            }
            if (FilesChanged != null)
                FilesChanged(this, EventArgs.Empty);
            
            
        }

        #endregion

        /// <summary>
        /// Generate the list of commands to rename the files
        /// </summary>
        /// <returns>A list of RenameCommands</returns>
        public List<RenameCommand> GenerateRenameCommands()
        {
            //GenerateNewFileNames();

            if (Clashes)
            {
                throw (new RenameClashException());
            }

            List<RenameCommand> commands = new List<RenameCommand>(_fileMetaData.Count);

            var mover = IOMove.Create();
            for (int i=0; i < _fileMetaData.Count; i++)
            {
                var rename = new RenameCommand(mover);
                rename.Initialize(_fileMetaData[i].Directory, _fileMetaData[i].Name, Files[i].NewFileName);
                commands.Add(rename);
            }

            return commands;
        }
    }

    /// <summary>
    /// Exception raised when we know that renaming would cause a clash.
    /// </summary>
    public class RenameClashException : Exception
    {
        public RenameClashException()
        {
        }

        public RenameClashException(string message)
            : base(message)
        {
        }

        public RenameClashException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
