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
    class CaseChangingViewModel: StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        string[] _caseOptions;
        CaseChangingStrategy _strategy;

        #endregion

        #region Constructor

        public CaseChangingViewModel()
        {
            _strategy = new CaseChangingStrategy(CaseTypes.Lowercase);
            DisplayName = "Change Case";
        }

        #endregion

        #region Properties

        override public IFileRenamerStrategy Strategy
        {
            get { return _strategy; }
        }

        public string[] CaseOptions
        {
            get
            {
                if (_caseOptions == null)
                {
                    _caseOptions = new string[] {"lowercase", "UPPERCASE", "Camelcase", "Sentence case"};
                }
                return _caseOptions;
            }
        }

        public int Case
        {
            get { return (int)_strategy.CaseType; }
            set
            {
                if (value >= 0 && value < Enum.GetValues(typeof(CaseTypes)).Length)
                {
                    _strategy.CaseType = (CaseTypes)value;
                    base.OnPropertyChanged("Case");
                }
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
                    // The IsCompany property of the Customer class 
                    // is Boolean, so it has no concept of being in
                    // an "unselected" state.  The CustomerViewModel
                    // class handles this mapping and validation.
                    error = this.ValidatePosition();
                }
                else
                {
                    //error = (_customer as IDataErrorInfo)[propertyName];
                    error = null;
                }

                // Dirty the commands registered with CommandManager,
                // such as our Save command, so that they are queried
                // to see if they can execute now.
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
