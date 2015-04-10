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
        //private string _directory;

        /// <summary>
        /// The strategy used to rename a file
        /// </summary>
        public IFileRenamer RenameStrategy;

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
            for (int i = 0; i < _fileMetaData.Count; i++)
            {
                _newFileNames.Add(RenameStrategy.RenameFile(_fileMetaData[i], i));
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
            RenameStrategy = new IdentityStrategy();

            CheckForClashes();
        }
    }

    /// <summary>
    /// Represents a file. Contains name, directory, and any other metadata used when renaming a file.
    /// </summary>
    public class FileMetaData
    {
        private String _name;
        private String _directory;
        private FileInfo _file;

        public String Directory
        {
            get { return _directory; }
        }

        public String Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Is this entry a duplicate?
        /// </summary>
        public bool IsDuplicate;

        /// <summary>
        /// Create a FileMetaData Object.  In the future this will automatically load meta data from the given file.
        /// </summary>
        /// <param name="Name">The file name</param>
        public FileMetaData (String Name)
        {
            _file = new FileInfo(Name);
            _directory = _file.DirectoryName;
            _name = _file.Name;
        }

    }

    /// <summary>
    /// This interface encapsulates a strategy to rename a single file.
    /// </summary>
    public interface IFileRenamer
    {
        String RenameFile(FileMetaData FileName, int Position);
    }

    /// <summary>
    /// Just return the name unchanged.
    /// </summary>
    public class IdentityStrategy : IFileRenamer
    {

        string IFileRenamer.RenameFile(FileMetaData FileName, int Position)
        {
            return FileName.Name;
        }
    }

    public interface ICommand
    {
        void Run();
        void Undo();
    }

    public class RenameCommand : ICommand
    {
        private string _originalName;
        private string _newName;
        private string _directory;
        private bool _hasRun;

        public string OriginalName
        {
            get { return _originalName; }
        }
        
        public string NewName
        {
            get { return _newName; }
        }
        
        public string Directory
        {
            get { return _directory; }
        }

        public RenameCommand(string Directoy, string OldName, string NewName)
        {
            _originalName = OldName;
            _newName = NewName;
            _directory = Directory;
            _hasRun = false;
        }

        public void Run()
        {
            _hasRun = true;
            File.Move(_directory + _originalName, _directory + _newName);
        }

        public void Undo()
        {
            if (_hasRun)
            {
                _hasRun = false;
                File.Move(_directory + _newName, _directory + _originalName);
            }

        }
    }

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
