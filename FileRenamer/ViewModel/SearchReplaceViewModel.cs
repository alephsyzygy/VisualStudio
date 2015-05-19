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
    class SearchReplaceViewModel: StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        SearchReplaceStrategy _strategy;

        #endregion

        #region Constructor

        public SearchReplaceViewModel()
        {
            _strategy = new SearchReplaceStrategy("","",false,false);
            DisplayName = "Search & Replace";
        }

        #endregion

        #region Properties

        override public IFileRenamerStrategy Strategy
        {
            get { return _strategy; }
        }

        public string SearchRegex
        {
            get { return _strategy.SearchRegex; }
            set
            {
                _strategy.SearchRegex = value;
                base.OnPropertyChanged("SearchRegex");

            }
        }

        public string ReplaceRegex
        {
            get { return _strategy.ReplaceRegex; }
            set
            {
                _strategy.ReplaceRegex = value;
                base.OnPropertyChanged("ReplaceRegex");

            }
        }

        public bool UseRegex
        {
            get { return _strategy.UseRegex; }
            set
            {
                _strategy.UseRegex = value;
                base.OnPropertyChanged("UseRegex");

            }
        }

        public bool CaseSensitive
        {
            get { return  _strategy.CaseSensitive; }
            set
            {
                _strategy.CaseSensitive = value;
                base.OnPropertyChanged("CaseSensitive");

            }
        }

        #endregion
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            { //return (_customer as IDataErrorInfo).Error; }
                return null;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string error = null;

                if (propertyName == "Position")
                {
                    error = this.ValidatePosition();
                }
                else
                {
                    error = null;
                }

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
