using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileRenamer.Model
{
    /// <summary>
    /// Object containing EXIFData extracted from a file
    /// </summary>
    public class EXIFData
    {
        // Ideas adapted from http://www.developerfusion.com/article/84474/reading-writing-and-photo-metadata/

        public DateTime DateTaken { get; private set; }

        private EXIFData()
        { }

        /// <summary>
        /// Read a filename for EXIF data.  If no data is found null is returned.
        /// </summary>
        /// <param name="Filename">The filename to open</param>
        /// <returns>An EXIFData object if such data exists</returns>
        public static EXIFData Read(string Filename)
        {
            try
            {
                using (FileStream stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    JpegBitmapDecoder jpegDecoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    EXIFData data = new EXIFData();
                    BitmapMetadata metadata = (BitmapMetadata)jpegDecoder.Frames[0].Metadata;
                    DateTime taken = DateTime.Parse(metadata.DateTaken);
                    data.DateTaken = taken;

                    return data;
                }
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException || e is ArgumentException || e is NotSupportedException || e is FileFormatException)
                {
                    return null;
                }
                throw;
            
            }

        }
    }
}
