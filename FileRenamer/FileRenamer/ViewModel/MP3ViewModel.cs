using FileRenamer.Strategies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileRenamer.ViewModel
{
    class MP3ViewModel : StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        MP3Strategy _strategy;

        #endregion

        #region Constructor

        public MP3ViewModel()
        {
            _strategy = new MP3Strategy("%n");
            DisplayName = "Insert MP3 Data";
        }

        #endregion

        #region Properties

        override public IFileRenamerStrategy Strategy
        {
            get { return _strategy; }
        }

        public string FormatString
        {
            get { return _strategy.FormatString; }
            set
            {
                _strategy.FormatString = value;

                base.OnPropertyChanged("FormatString");
            }
        }


        #endregion
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            { 
                return null;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;

                CommandManager.InvalidateRequerySuggested();

                return error;
            }
        }

        string ValidatePosition()
        {
            return null;
        }

        #endregion // IDataErrorInfo Members


    }
}