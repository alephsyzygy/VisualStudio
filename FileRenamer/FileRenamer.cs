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
        private IFileRenamer _renameStrategy;
        //private string _directory;

        /// <summary>
        /// The strategy used to rename a file
        /// </summary>
        public IFileRenamer RenameStrategy
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
            for (int i = 0; i < _fileMetaData.Count; i++)
            {
                _newFileNames.Add(_renameStrategy.RenameFile(_fileMetaData[i], i));
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

    /// <summary>
    /// Different behaviours for dealing with suffixes:
    ///   NameOnly: only modify the name
    ///   SuffixOnly: only modify the suffix
    ///   BothNameSuffix: modify anything in the filename, including the separator
    /// The separator is the last period "." found in a file name, if it has one.
    /// The suffix is anything after the separator and the name is anything before.
    /// If a file name does not have a separator then the name is the whole file name, and the suffix is empty
    /// </summary>
    public enum NameSuffixBehaviour
    {
        NameOnly,
        SuffixOnly,
        BothNameSuffix
    };

    /// <summary>
    /// A class to help deal with names and suffixes.
    /// </summary>
    public class NameSuffixHelper
    {
        private string _text;
        private bool _hasSeparator;
        private string _name;
        private string _suffix;
        private int _suffixPos;
        private NameSuffixBehaviour _behaviour;

        /// <summary>
        /// Where in the original text the suffix starts
        /// </summary>
        public int SuffixPos
        {
            get { return _suffixPos;  }
        }

        /// <summary>
        /// Does the filename have a separator
        /// </summary>
        public bool HasSeparator
        {
            get { return _hasSeparator; }
        }

        /// <summary>
        /// The suffix of the file name
        /// </summary>
        public string Suffix
        {
            get { return _suffix; }
        }

        /// <summary>
        ///  The name part of the file name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The whole text of the file name
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
        }

        /// <summary>
        /// The separator string of the file name.
        /// It is "." if a file has a separator, otherwise it is empty.
        /// </summary>
        public string SeparatorString
        {
            get
            {
                if (_hasSeparator)
                {
                    return ".";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Text">The filename</param>
        /// <param name="Behaviour">What behaviour to use for inserting and overwriting</param>
        public NameSuffixHelper(string Text, NameSuffixBehaviour Behaviour)
        {
            _behaviour = Behaviour;
            _text = Text;
            _hasSeparator = _text.Contains(".");
            int finalDotPos = _text.LastIndexOf(".");
            _suffix = _text.Substring(finalDotPos + 1);
            if (finalDotPos >= 0)
            {
                _name = _text.Substring(0, finalDotPos);
                _suffixPos = finalDotPos;
            }
            else
            {
                _name = _text;
                _suffixPos = _text.Length;
            }
        }

        /// <summary>
        /// Insert some text into the given position, with behaviour given by the constructor.
        /// If the position is greater the length then it appends.
        /// </summary>
        /// <param name="Position">Position to insert</param>
        /// <param name="Text">Text to insert</param>
        /// <returns>New filename</returns>
        public string Insert(int Position, string Text)
        {
            string newName;
            string newSuffix;

            switch (_behaviour)
            {
                case NameSuffixBehaviour.NameOnly:
                    newName = _name.Insert(Math.Min(Position, _name.Length), Text);
                    newSuffix = _suffix;
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.SuffixOnly:
                    newName = _name;
                    newSuffix = _suffix.Insert(Math.Min(Position, _suffix.Length), Text);
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.BothNameSuffix:
                    goto default;
                default:
                    return _text.Insert(Math.Min(Position, _text.Length), Text);
            }
        }

        /// <summary>
        /// Overwrite parts of the filename with the given text, at the given position.
        /// The name/suffix behaviour is determined by the constructor.
        /// If the text to overwrite overruns the original text then the length is extended.
        /// If the position is greater than the length of the text then it appends.
        /// </summary>
        /// <param name="Position">Position to overwrite</param>
        /// <param name="Text">Text to overwrite</param>
        /// <returns>New filename</returns>
        public string Overwrite(int Position, string Text)
        {
            string newName;
            string newSuffix; 
            
            switch (_behaviour)
            {
                case NameSuffixBehaviour.NameOnly:
                    newName = OverwriteOrAppend(_name, Position, Text);
                    newSuffix = _suffix;
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.SuffixOnly:
                    newName = _name;
                    newSuffix = OverwriteOrAppend(_suffix, Position, Text);
                    return newName + SeparatorString + newSuffix;
                case NameSuffixBehaviour.BothNameSuffix:
                    goto default;
                default:
                    return OverwriteOrAppend(_text, Position, Text);
            }
        }

        /// <summary>
        /// Internal method to perform the overwrite.
        /// </summary>
        /// <param name="InputText">Inital text to overwrite</param>
        /// <param name="Position">Position to overwrite</param>
        /// <param name="OverwriteText">Text we overwrite with</param>
        /// <returns>String</returns>
        private string OverwriteOrAppend(string InputText, int Position, string OverwriteText)
        {
            var stringBuilder = new StringBuilder(InputText);
            // Find the right amount of characters to remove.  If we are overwriting with a string longer than
            // what remains then we just append to the end.
            var lengthToRemove = Math.Min(OverwriteText.Length, Math.Max(0,InputText.Length - Position));
            if (lengthToRemove > 0)
            {
                stringBuilder.Remove(Position, lengthToRemove);
            }
            stringBuilder.Insert(Position, OverwriteText);
            return stringBuilder.ToString();
        }
    }

    public class InsertTextStrategy : IFileRenamer
    {
        private int _position;
        private string _text;
        private bool _insert;
        private NameSuffixBehaviour _behaviour;

        public InsertTextStrategy(int Position, string Text, bool InsertOrOverwrite, NameSuffixBehaviour Behaviour)
        {
            _text = Text;
            _position = Position;
            _insert = InsertOrOverwrite;
            _behaviour = Behaviour;
        }

        public string RenameFile(FileMetaData FileName, int Position)
        {
            string newName;
            NameSuffixHelper nameSuffix = new NameSuffixHelper(FileName.Name, _behaviour);

            if (_insert)
            {
                newName = nameSuffix.Insert(_position, _text);
            }
            else
            {
                newName = nameSuffix.Overwrite(_position, _text);
            }

            return newName;
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
