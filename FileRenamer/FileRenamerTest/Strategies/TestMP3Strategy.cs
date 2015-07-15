using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Model;
using FileRenamer.Strategies;
using Moq;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestMP3Strategy
    {
        private ID3Tag tag;

        // Create an ID3Tag
        void CreateTag()
        {
            PrivateObject accessor = new PrivateObject(typeof (ID3Tag));
            accessor.SetFieldOrProperty("Artist", "Artist");
            accessor.SetFieldOrProperty("Album", "Album");
            accessor.SetFieldOrProperty("Title", "Title");
            accessor.SetFieldOrProperty("Comment", "Comment");
            accessor.SetFieldOrProperty("Year", "Year");
            accessor.SetFieldOrProperty("Genre", "Genre");
            tag = (ID3Tag) accessor.Target;
        }

        // Use Moq to create an IFileMetaData object
        private IFileMetaData CreateFileMetaData(string name)
        {
            CreateTag();
            Mock<IFileMetaData> mock = new Mock<IFileMetaData>();
            mock.SetupGet(m => m.Name).Returns(name);
            mock.SetupGet(m => m.ID3Tag).Returns(tag);
            return mock.Object;
        }

        [TestMethod]
        public void TestMp3Data()
        {
            string format = "Testing - %a - %b - %c - %g - %y - %%.mp3";
            string expected = "Testing - Artist - Album - Comment - Genre - Year - %.mp3";

            var strategy = new MP3Strategy(format);
            var helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
            string result = strategy.RenameFile(CreateFileMetaData("test"), 0, helper);

            Assert.AreEqual(expected, result);

            // Now test other parts
            format = "Position test %p.mp3";
            expected = "Position test 99.mp3";
            strategy.FormatString = format;
            result = strategy.RenameFile(CreateFileMetaData("test"), 99, helper);
            Assert.AreEqual(expected, result);


            // Name and extension
            format = "Extension test %f - %n - %x";
            expected = "Extension test test.mp3 - test - mp3.mp3";
            strategy.FormatString = format;
            result = strategy.RenameFile(CreateFileMetaData("test.mp3"), 99, helper);
            Assert.AreEqual(expected, result);
        }
    }
}
