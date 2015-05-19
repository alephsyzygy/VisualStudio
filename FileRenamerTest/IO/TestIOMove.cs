using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.IO;
using System.IO;

namespace FileRenamerTest.IO
{
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

    [TestClass]
    public class TestIOMove
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
