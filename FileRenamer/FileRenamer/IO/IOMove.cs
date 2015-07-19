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
        public IIOMove IIOMove
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
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

}
