using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.ViewModel
{
    class FileListViewModel : ViewModelBase
    {
        #region Fields

        RenamerModel _fileRenamerModel;
        CommandViewModel _move;

        #endregion

        #region Constructor

        public FileListViewModel(RenamerModel FileRenamerModel, CommandViewModel Move)
        {
            _fileRenamerModel = FileRenamerModel;
            _fileRenamerModel.Files.CollectionChanged += OnCollectionChanged;
            _fileRenamerModel.StrategyChanged += OnStrategyChanged;

            _move = Move;

            AllFiles = new ObservableCollection<FileViewModel>();

            GenerateFiles();
        }

        #endregion

        #region Properties

        public ObservableCollection<FileViewModel> AllFiles { get; private set; }

        #endregion

        #region Commands

        public CommandViewModel Move { get { return _move; } }

        #endregion

        #region Events

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GenerateFiles();
        }

        public void OnStrategyChanged(object sender, EventArgs e)
        {
            GenerateFiles();
        }

        #endregion

        #region Private Helpers

        private void GenerateFiles()
        {
            
            AllFiles.Clear();
            foreach (var file in _fileRenamerModel.Files)
            {
                AllFiles.Add(new FileViewModel(file));
            }
        }

        #endregion

    }
}
