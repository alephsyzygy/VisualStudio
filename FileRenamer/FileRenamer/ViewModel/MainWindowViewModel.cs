using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Strategies;
using System.Windows;
using FileRenamer.IO;

namespace FileRenamer.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        ReadOnlyCollection<CommandViewModel> _commands;
        readonly RenamerModel _renamerModel;
        ObservableCollection<StrategyViewModel> _strategies;
        int _selectedStrategy;
        string[] _nameExtensionOptions;
        int _selectedNameExtensionHelper;
        readonly NameExtensionHelper[] _helpers;
        CommandViewModel _move;

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel(string[] Files)
        {
            //base.DisplayName = Strings.MainWindowViewModel_DisplayName;

            //_customerRepository = new CustomerRepository(customerDataFile);
            _helpers = new NameExtensionHelper[]{
                NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly),
                NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.ExtensionOnly),
                NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.BothNameExtension)
            };

            _renamerModel = new RenamerModel(Files.Select(name => (IFileMetaData) new FileMetaData(name)).ToList());

            _move = new CommandViewModel("Move", new RelayCommand(param => this.Move(param)));

            FileList = new FileListViewModel(_renamerModel, _move);
            _renamerModel.StrategyChanged += FileList.OnStrategyChanged;
            _renamerModel.FilesChanged += FileList.OnStrategyChanged;
            _renamerModel.Helper = _helpers[0];
            SelectedStrategy = 0;
        }

        #endregion

        #region Properties

        public FileListViewModel FileList { get; private set; }

        public string[] NameExtensionOptions
        {
            get
            {
                if (_nameExtensionOptions == null)
                {
                    _nameExtensionOptions = new string[]
                    {
                        "Name Only",
                        "Extension Only",
                        "Name & Extension"
                    };
                }
                return _nameExtensionOptions;
            }
        }

        public int NameExtensionSelection
        {
            get
            {
                return _selectedNameExtensionHelper;
            }
            set
            {
                if (value >= 0 && value < _helpers.Count())
                {
                    _selectedNameExtensionHelper = value;
                    _renamerModel.Helper = _helpers[_selectedNameExtensionHelper];
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    "Quit",
                    new RelayCommand(param => this.CloseWindow(param))),
                new CommandViewModel(
                    "Rename Files",
                    new RelayCommand(param => this.RenameFiles()))
            };
        }

        #endregion // Commands

        #region Strategies

        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<StrategyViewModel> Strategies
        {
            get
            {
                if (_strategies == null)
                {
                    _strategies = new ObservableCollection<StrategyViewModel>
                    {
                        new InsertTextViewModel(),
                        new RemoveCharactersViewModel(),
                        new NumberingViewModel(),
                        new CaseChangingViewModel(),
                        new DateInserterViewModel(),
                        new SearchReplaceViewModel(),
                        new MP3ViewModel()
                    };

                    //_strategies.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _strategies;
            }
        }

        public int SelectedStrategy
        {
            get {return _selectedStrategy;}
            set
            {
                _renamerModel.RenameStrategy = Strategies[value].Strategy;
                _selectedStrategy = value;
            }
        }

        #endregion Strategies

        #region Private Helpers

        void Move(Object param)
        {
            var data = param as Tuple<int, int>;
            if (data != null)
            {
                _renamerModel.Move(data.Item1, data.Item2);
            }
        }


        void RenameFiles()
        {
                        
            if (!_renamerModel.Clashes)
            {
                RenameAllCommand renameAllCommand = new RenameAllCommand(_renamerModel.GenerateRenameCommands(), showDialog);

                renameAllCommand.Run();

                if (renameAllCommand.Successful)
                {
                    MessageBox.Show("Files renamed.");
                        
                }
                else
                {
                    MessageBox.Show("File renaming cancelled.");
                }

                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Cannot rename files since some clash.");
            }

        }

        private RenameFailureBehaviour showDialog()
        {
            ErrorDialog errorDialog = new ErrorDialog();
            if (errorDialog.ShowDialog() == true)
            {
                return (RenameFailureBehaviour)errorDialog.Result;
            }
            else
            {
                return RenameFailureBehaviour.Undo;
            }
        }

        void Quit()
        {
            //Application.Current.Shutdown();
            //this.
        }

        // close this window
        private void CloseWindow(object window)
        {
            if (window != null && window is Window)
            {
                ((Window)window).Close();
            }
        }

        void Move(Tuple<int,int> Pair)
        {

        }

        #endregion
    }
}
