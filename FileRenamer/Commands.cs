using System.IO;
namespace FileRenamer
{
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
}