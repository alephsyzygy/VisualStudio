using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.IO
{
    /// <summary>
    /// An IIOCommand is an interface for an object which implements a run
    /// action and an undo action.  Objects which implemenent this interface
    /// are intended to interact with the filesystem.
    /// </summary>
    public interface IIOCommand
    {
        void Run();
        void Undo();
    }

    /// <summary>
    /// RenameCommand is an object which does the renaming of a single file.
    /// </summary>
    public class RenameCommand : IIOCommand
    {
        private string _originalName;
        private string _newName;
        private string _directory;
        private bool _hasRun;
        private IIOMove _mover;

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Mover">Object which will perform the file move operation</param>
        public RenameCommand(IIOMove Mover)
        {
            _mover = Mover;
        }

        /// <summary>
        /// Initialize the necessary parameters for this object
        /// </summary>
        /// <param name="Directory">The directory the file is localed</param>
        /// <param name="OldName">The old filename</param>
        /// <param name="NewName">The new filename</param>
        public void Initialize(string Directory, string OldName, string NewName)
        {
            _originalName = OldName;
            _newName = NewName;
            _directory = Directory;
            _hasRun = false;
        }

        /// <summary>
        /// Perform the renaming
        /// </summary>
        public void Run()
        {
            _hasRun = true;
            _mover.Move(_directory + Path.DirectorySeparatorChar + _originalName, 
                _directory + Path.DirectorySeparatorChar + _newName);
        }

        /// <summary>
        /// Undo the renaming.
        /// If the file has not been renamed previously then this method does nothing
        /// </summary>
        public void Undo()
        {
            if (_hasRun)
            {
                _hasRun = false;
                _mover.Move(_directory + Path.DirectorySeparatorChar + _newName, 
                    _directory + Path.DirectorySeparatorChar + _originalName);
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
    /// Command which does all the renaming.
    /// Has to be given a list of RenameCommands and an action to take when an error occurs.
    /// This action must return a RenameFailureBehaviour to indicate what error handling behaviour
    /// should be taken.
    /// </summary>
    public class RenameAllCommand : IIOCommand
    {
        private List<RenameCommand> _commands;
        private bool _hasRun;
        private int _pos;
        private RenameFailureBehaviour _behaviour;
        // Note: Func<RenameFailureBehaviour> is a function which takes no inputs,
        //  but returns a RenameFailureBehaviour.
        private Func<RenameFailureBehaviour> _showDialog;
        private bool _successful;
        private List<bool> _skipped;

        /// <summary>
        /// Has the RenameAllCommand been run successfully?
        /// </summary>
        public bool Successful
        {
            get { return _successful; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Commands">List of RenameCommand's to perform</param>
        /// <param name="ShowDialog">Action to take in the event of an exception</param>
        public RenameAllCommand(List<RenameCommand> Commands, Func<RenameFailureBehaviour> ShowDialog)
        {
            _commands = Commands;
            _hasRun = false;
            _pos = 0;
            // initial behaviour when an error occurs is to show a dialog, i.e. run ShowDialog
            _behaviour = RenameFailureBehaviour.Dialog;
            _showDialog = ShowDialog;
            _successful = false;
            _skipped = new List<bool>(Enumerable.Repeat(false, Commands.Count));
        }

        /// <summary>
        /// Run the RenameAllCommand.
        /// This attempts to run all the rename commands given
        /// in the constructor.
        /// In the event of an error it will run the ShowDialog Func
        /// </summary>
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
                    catch (Exception)  // TODO: make the exception caught more specific
                    {
                        // Some filesystem error has occurred
                        _pos = i;
                        if (_behaviour == RenameFailureBehaviour.Dialog)
                        {
                            _behaviour = _showDialog();
                        }
                        switch (_behaviour)
                        {
                            case RenameFailureBehaviour.Dialog:
                                // This case should not happen
                                goto default;
                            case RenameFailureBehaviour.Undo:
                                // User wants to undo everything
                                _hasRun = true;
                                Undo();
                                return;
                            case RenameFailureBehaviour.Skip:
                                // User wants to skip this error
                                _behaviour = RenameFailureBehaviour.Dialog;
                                _skipped[i] = true;
                                break;
                            case RenameFailureBehaviour.SilentContinue:
                                // User wants to skip all errors
                                break;
                            case RenameFailureBehaviour.Abort:
                                // User wants to stop immediately
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

        /// <summary>
        /// Undo the actions we have taken so far
        /// </summary>
        public void Undo()
        {
            if (_hasRun)
            {
                for (int i = _pos - 1; i >= 0; i--)
                {
                    // Note: if the user skipped a file due to an error we
                    //   do not undo the action, since it was never successfully
                    //   completed in the first case
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
