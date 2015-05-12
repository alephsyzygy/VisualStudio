using System;
using System.IO;

namespace FileRenamer.Model
{

    /// <summary>
    /// Represents a file. Contains name, directory, and any other metadata used when renaming a file.
    /// </summary>
    public class FileMetaData
    {
        private String _name;
        private String _directory;
        private FileInfo _file;
        private DateTime _modified;
        private DateTime _created;
        private bool _isMP3;
        private ID3Tag _tag;

        public String Directory
        {
            get { return _directory; }
        }

        public String Name
        {
            get { return _name; }
        }

        public DateTime Modified { get { return _modified;  } }

        public DateTime Created { get { return _created; } }

        /// <summary>
        /// Is this entry a duplicate?
        /// </summary>
        public bool IsDuplicate;

        /// <summary>
        /// Create a FileMetaData Object.  In the future this will automatically load meta data from the given file.
        /// </summary>
        /// <param name="Name">The file name</param>
        public FileMetaData(String Name)
        {
            _file = new FileInfo(Name);
            _directory = _file.DirectoryName;
            _name = _file.Name;
            _modified = File.GetLastWriteTime(Name);
            _created = File.GetCreationTime(Name);
            _isMP3 = false;
            _tag = ID3Tag.Read(Name);
            if (_tag != null)
            {
                _isMP3 = true;
            }
        }

    }
}