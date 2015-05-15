using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer;
using Ninject;

namespace FileRenamer.IO
{
    /// <summary>
    /// Interface representing the File.Move static method
    /// Instead of using File.Move directly we use this interface
    /// so that we can test the various rename commands without touching
    /// the filesystem.
    /// </summary>
    public interface IIOMove
    {
        void Move(string sourceFileName, string destFileName);
    }

    public class IOMove : IIOMove
    {
        /// <summary>
        /// Move a file from one place to another.
        /// This just calls the static method File.Move
        /// </summary>
        /// <param name="sourceFileName">Original FileName</param>
        /// <param name="destFileName">New FileName</param>
        public void Move(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        /// <summary>
        /// Factory method to create an instance
        /// </summary>
        /// <returns></returns>
        public static IIOMove Create()
        {
            return Kernel.kernel.Get<IIOMove>();
        }
    }

    /// <summary>
    /// IOMoveLogger does not perform any moves, instead it logs all moves
    /// to a text file.
    /// </summary>
    public class IOMoveLogger : IIOMove
    {
        const string LogFile = "IOMoveLogger.txt";

        public IOMoveLogger()
        {
            using (TextWriter logger = File.AppendText(LogFile))
            {
                logger.WriteLine(DateTime.Now.ToString());
            }
        }

        public void Move(string sourceFileName, string destFileName)
        {
            using (TextWriter logger = File.AppendText(LogFile))
            {
                logger.WriteLine(sourceFileName + " --> " + destFileName);
            }
        }
    }
}
