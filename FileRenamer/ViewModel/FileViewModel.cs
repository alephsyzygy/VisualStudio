using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Model;

namespace FileRenamer.ViewModel
{
    class FileViewModel : ViewModelBase
    {
        #region Fields

        FileModel _file;

        #endregion

        #region Constructor

        public FileViewModel(FileModel File)
        {
            _file = File;
        }

        #endregion

        #region Properties

        public string OriginalFileName { get { return _file.OriginalFileName; } }
        public string NewFileName { get { return _file.NewFileName; } }
        public bool Clashes { get { return _file.Clashes; } }

        #endregion

    }
}
