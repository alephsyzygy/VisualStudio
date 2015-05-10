using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.IO
{
    public interface IIOCommand
    {
        void Run();
        void Undo();
    }

    public class RenameCommand : IIOCommand
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

        public RenameCommand(string Directory, string OldName, string NewName)
        {
            _originalName = OldName;
            _newName = NewName;
            _directory = Directory;
            _hasRun = false;
        }

        public void Run()
        {
            _hasRun = true;
            File.Move(_directory + Path.DirectorySeparatorChar + _originalName, _directory + Path.DirectorySeparatorChar + _newName);
        }

        public void Undo()
        {
            if (_hasRun)
            {
                _hasRun = false;
                File.Move(_directory + Path.DirectorySeparatorChar + _newName, _directory + Path.DirectorySeparatorChar + _originalName);
            }

        }
    }

    /// <summary>
    /// Possible failure modes when renaming.
    /// </summary>
    public enum RenameFailureBehaviour
    {
        Dialog,         // Show a dialog
        Undo,           // Undo everything
        Skip,           // Skip this file
        SilentContinue, // Skip all errors
        Abort           // Abort, do no cleaning up
    }

    /// <summary>
    /// Command which does all the remaining.
    /// Has to be given a list of RenameCommands and an action to take when an error occurs.
    /// This action returns a RenameFailureBehaviour.
    /// </summary>
    public class RenameAllCommand : IIOCommand
    {
        private List<RenameCommand> _commands;
        private bool _hasRun;
        private int _pos;
        private RenameFailureBehaviour _behaviour;
        private Func<RenameFailureBehaviour> _showDialog;
        private bool _successful;
        private List<bool> _skipped;

        public bool Successful
        {
            get { return _successful; }
        }

        public RenameAllCommand(List<RenameCommand> Commands, Func<RenameFailureBehaviour>

ShowDialog)
        {
            _commands = Commands;
            _hasRun = false;
            _pos = 0;
            _behaviour = RenameFailureBehaviour.Dialog;
            _showDialog = ShowDialog;
            _successful = false;
            _skipped = new List<bool>(Enumerable.Repeat(false, Commands.Count));
        }

        public void Run()
        {
            if (!_hasRun)
            {
                for (int i = 0; i < _commands.Count; i++)
                {
                    try
                    {
                        _commands[i].Run();
                    }
                    catch (Exception)
                    {
                        _pos = i;
                        if (_behaviour == RenameFailureBehaviour.Dialog)
                        {
                            _behaviour = _showDialog();
                        }
                        switch (_behaviour)
                        {
                            case RenameFailureBehaviour.Dialog:
                                goto default;
                            case RenameFailureBehaviour.Undo:
                                _hasRun = true;
                                Undo();
                                return;
                            case RenameFailureBehaviour.Skip:
                                _behaviour = RenameFailureBehaviour.Dialog;
                                _skipped[i] = true;
                                break;
                            case RenameFailureBehaviour.SilentContinue:
                                break;
                            case RenameFailureBehaviour.Abort:
                                return;
                            default:
                                throw new Exception("Dialog should return a result");
                        }
                    }

                }
                _hasRun = true;
                _successful = true;
            }
        }


        public void Undo()
        {
            if (_hasRun)
            {
                for (int i = _pos - 1; i >= 0; i--)
                {
                    if (!_skipped[i])
                    {
                        _commands[i].Undo();
                    }
                }
                _hasRun = false;
                _successful = false;
            }
        }
    }

}
