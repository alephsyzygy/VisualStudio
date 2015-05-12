using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Model
{
    /// <summary>
    /// Reprents an ID3v1 tag of a MP3 file
    /// </summary>
    public class ID3Tag
    {
        // Adapted from an answer to http://stackoverflow.com/questions/68283/view-edit-id3-data-for-mp3-files
        // This could be handled by an external library, but this lets me experiment with filestreams.

        /// <summary>
        /// Internal class representing the byte structure of an ID3 tag
        /// </summary>
        private class ID3TagByte
        {
            public byte[] TAGID = new byte[3];
            public byte[] Title = new byte[30];
            public byte[] Artist = new byte[30];
            public byte[] Album = new byte[30];
            public byte[] Year = new byte[4];
            public byte[] Comment = new byte[30];
            public byte[] Genre = new byte[1];
        }

        #region Public Fields

        public string Title;
        public string Artist;
        public string Album;
        public string Year;
        public string Comment;
        public string Genre;

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor is private, to construct an instance use the static method Read
        /// </summary>
        private ID3Tag()
        {}

        #endregion Constructor

        #region Static Methods

        /// <summary>
        /// Read a file and return an ID3Tag constructed from the file.
        /// If an ID3Tag does not exist null is returned.
        /// </summary>
        /// <param name="Filename">The name of the file to read from</param>
        /// <returns>The ID3Tag contained in the file, or null if none exists</returns>
        public static ID3Tag Read(string Filename)
        {
            try
            {
                using (FileStream fs = File.OpenRead(Filename))
                {
                    // The ID3v1 tag is at the end of the file and is always 128 bytes long
                    if (fs.Length >= 128)
                    {
                        ID3TagByte tag = new ID3TagByte();
                        fs.Seek(-128, SeekOrigin.End);
                        fs.Read(tag.TAGID, 0, tag.TAGID.Length);
                        fs.Read(tag.Title, 0, tag.Title.Length);
                        fs.Read(tag.Artist, 0, tag.Artist.Length);
                        fs.Read(tag.Album, 0, tag.Album.Length);
                        fs.Read(tag.Year, 0, tag.Year.Length);
                        fs.Read(tag.Comment, 0, tag.Comment.Length);
                        fs.Read(tag.Genre, 0, tag.Genre.Length);
                        string theTAGID = Encoding.Default.GetString(tag.TAGID);

                        // The tag ID should be TAG in order for this to be a proper ID3v1 tag
                        if (theTAGID.Equals("TAG"))
                        {
                            ID3Tag outputTag = new ID3Tag();
                            outputTag.Title = Encoding.Default.GetString(tag.Title).Trim();
                            outputTag.Artist = Encoding.Default.GetString(tag.Artist).Trim();
                            outputTag.Album = Encoding.Default.GetString(tag.Album).Trim();
                            outputTag.Year = Encoding.Default.GetString(tag.Year).Trim();
                            outputTag.Comment = Encoding.Default.GetString(tag.Comment).Trim();
                            outputTag.Genre = Encoding.Default.GetString(tag.Genre).Trim();
                            return outputTag;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (AccessViolationException)
            {
                return null;
            }

            return null;
        }

        #endregion

    }

}
