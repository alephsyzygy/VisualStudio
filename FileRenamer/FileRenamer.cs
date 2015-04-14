using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileRenamer
{
    /// <summary>
    /// BulkRenamer class.  Contains a list of file metadata and can construct a list of commands for renaming those files.
    /// </summary>
    public class BulkRenamer 
    {
        private ObservableCollection<FileMetaData> _fileMetaData;
        private List<String> _newFileNames;
        private bool _clashes;
        private IFileRenamerStrategy _renameStrategy;
        private ObservableCollection<Tuple<String, String, bool>> _collection;

        public ObservableCollection<Tuple<String,String,bool>> Collection
        { get { return _collection; } }

        //private string _directory;

        /// <summary>
        /// The strategy used to rename a file
        /// </summary>
        public IFileRenamerStrategy RenameStrategy
        {
            get
            {
                return _renameStrategy; 
            }
            set
            {
                _renameStrategy = value;
                CheckForClashes();
            }
        }

        public bool Clashes
        {
            get
            {
                return _clashes;
            }
        }

        /// <summary>
        /// Indexer for visualising the entries.
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>Tuple with original name, new name, and if it clashes</returns>
        public Tuple<String,String,bool> this[int i]
        {
            get
            {
                return Tuple.Create<String, String, bool>(_fileMetaData[i].Name, _newFileNames[i], _fileMetaData[i].IsDuplicate);
            }
        }

        public IEnumerable<Tuple<String,String,bool>> GetEnumerator()
        {
            for (int i = 0; i < _fileMetaData.Count; i++)
            {
                yield return Tuple.Create<String, String, bool>(_fileMetaData[i].Name, _newFileNames[i], _fileMetaData[i].IsDuplicate);
            }
        }

        /// <summary>
        /// Move an element in the list to another position
        /// </summary>
        /// <param name="from">index of element to move</param>
        /// <param name="to">index it moves to</param>
        public void Move(int from, int to)
        {
            _fileMetaData.Move(from, to);
            CheckForClashes();
        }

        private void GenerateNewFileNames()
        {
            _newFileNames = new List<string>(_fileMetaData.Count);
            _collection.Clear();
            for (int i = 0; i < _fileMetaData.Count; i++)
            {
                _newFileNames.Add(_renameStrategy.RenameFile(_fileMetaData[i], i));
                _collection.Add(Tuple.Create<String, String, bool>(_fileMetaData[i].Name, _newFileNames[i], _fileMetaData[i].IsDuplicate));
            }
        }

        private Boolean CheckForClashes()
        {
            // Clear all the duplicate flags in the meta data
            foreach (var metaData in _fileMetaData)
            {
                metaData.IsDuplicate = false;
            }

            GenerateNewFileNames();


            // Distinct() uses a hashtable internally so is O(n)
            bool clashes = _newFileNames.Count != _newFileNames.Distinct().Count();
            
            if (clashes)
            {
                // Mark all the elements that clash
                var duplicates = _fileMetaData.Zip(_newFileNames, (a,b) => Tuple.Create(a,b) ).GroupBy(x => x.Item2)
                        .Where(group => group.Count() > 1)
                        .SelectMany(group => group);
                foreach (var meta in duplicates)
                {
                    meta.Item1.IsDuplicate = true;
                }
            }

            _clashes = clashes;
            return clashes;
        }

        /// <summary>
        /// Generate the list of commands to rename the files
        /// </summary>
        /// <returns>A list of RenameCommands</returns>
        public List<RenameCommand> GenerateRenameCommands()
        {
            //GenerateNewFileNames();

            if (CheckForClashes())
            {
                throw (new RenameClashException());
            }

            List<RenameCommand> commands = new List<RenameCommand>(_fileMetaData.Count);

            int i = 0;
            for (i=0; i < _fileMetaData.Count; i++)
            {
                commands.Add(new RenameCommand(_fileMetaData[i].Directory, _fileMetaData[i].Name, _newFileNames[i]));
            }

            return commands;
        }

        /// <summary>
        /// Construct a BulkRenamer object.
        /// </summary>
        /// <param name="files">the list of file metadata</param>
        public BulkRenamer(List<FileMetaData> files)
        {
            _fileMetaData = new ObservableCollection<FileMetaData>(files);
            _newFileNames = new List<String>(_fileMetaData.Count);
                

            // We will default to the strategy that does nothing
            _renameStrategy = new IdentityStrategy();

            _collection = new ObservableCollection<Tuple<string,string,bool>>();

            CheckForClashes();
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
