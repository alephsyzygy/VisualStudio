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
    public class EXIFData
    {
        // Ideas adapted from http://www.developerfusion.com/article/84474/reading-writing-and-photo-metadata/

        public DateTime DateTaken { get; private set; }

        private EXIFData()
        { }

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
