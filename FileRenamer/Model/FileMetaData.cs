using System;
using System.IO;

namespace FileRenamer.Model
{
    public interface IFileMetaData
    {
        string Name { get; }
        string Directory { get; }
        DateTime Modified { get; }
        DateTime Created { get; }
        ID3Tag ID3Tag { get; }
        EXIFData EXIF { get; }
        bool IsDuplicate { get; set; } // Note: this can be set by a client
    }

    /// <summary>
    /// Represents a file. Contains name, directory, and any other metadata used when renaming a file.
    /// </summary>
    public class FileMetaData : IFileMetaData
    {
        private String _name;
        private String _directory;
        private FileInfo _file;
        private DateTime _modified;
        private DateTime _created;
        private bool _mp3tagRead;
        private ID3Tag _tag;
        private bool _exifRead;
        private EXIFData _exifData;

        /// <summary>
        /// The ID3Tag, if this represents an MP3 file
        /// </summary>
        public ID3Tag ID3Tag
        {
            get
            {
                // loaded on demand
                if (!_mp3tagRead)
                {
                    _mp3tagRead = true;
                    _tag = ID3Tag.Read(_file.FullName);
                    return _tag;
                }
                else
                {
                    return _tag;
                }
            }
        }

        /// <summary>
        /// The EXIFData, if this represents an image file
        /// </summary>
        public EXIFData EXIF
        {
            get
            {
                // loaded on demand
                if (!_exifRead)
                {
                    _exifRead = true;
                    _exifData = EXIFData.Read(_file.FullName);
                    return _exifData;
                }
                else
                {
                    return _exifData;
                }
            }
        }

        /// <summary>
        /// Directory of the file
        /// </summary>
        public String Directory
        {
            get { return _directory; }
        }

        /// <summary>
        /// Name of the file
        /// </summary>
        public String Name
        {
            get { return _name; }
        }

        /// <summary>
        /// When the file was last modified
        /// </summary>
        public DateTime Modified { get { return _modified;  } }

        /// <summary>
        /// When the file was created
        /// </summary>
        public DateTime Created { get { return _created; } }

        /// <summary>
        /// Is this entry a duplicate?
        /// </summary>
        public bool IsDuplicate { get; set; }

        /// <summary>
        /// Create a IFileMetaData Object.  In the future this will automatically load meta data from the given file.
        /// </summary>
        /// <param name="Name">The file name</param>
        public FileMetaData(String Name)
        {
            _file = new FileInfo(Name);
            _directory = _file.DirectoryName;
            _name = _file.Name;
            _modified = File.GetLastWriteTime(Name);
            _created = File.GetCreationTime(Name);
            _mp3tagRead = false;
            _exifRead = false;
        }

    }
}