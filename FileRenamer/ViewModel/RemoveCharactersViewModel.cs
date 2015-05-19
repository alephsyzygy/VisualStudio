using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenamer.Properties;
using System.Windows.Input;
using FileRenamer.Strategies;
using System.Collections.ObjectModel;

namespace FileRenamer.ViewModel
{
    class RemoveCharactersViewModel : StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        string _fromPos;
        string _toPos;
        string[] _locations;
        RemoveCharactersStrategy _strategy;

        #endregion

        #region Constructor

        public RemoveCharactersViewModel()
        {
            _fromPos = "0";
            _toPos = "0";
            _strategy = new RemoveCharactersStrategy(0, true,0, true);
            DisplayName = "Remove Characters";
        }

        #endregion

        #region Properties

        override public IFileRenamerStrategy Strategy
        {
            get { return _strategy; }
        }

        public string[] LocationOptions
        {
            get
            {
                if (_locations == null)
                {
                    _locations = new string[] { "From the left", "From the right" };
                }
                return _locations;
            }
        }

        public string FromPosition
        {
            get { return _strategy.FromPosition.ToString(); }
            set
            {
                _fromPos = value;

                int num = 0;
                bool parse = int.TryParse(_fromPos, out num);
                _strategy.FromPosition = num;

                base.OnPropertyChanged("FromPosition");
            }
        }

        public string ToPosition
        {
            get { return _strategy.ToPosition.ToString(); }
            set
            {
                _toPos = value;

                int num = 0;
                bool parse = int.TryParse(_toPos, out num);
                _strategy.ToPosition = num;

                base.OnPropertyChanged("ToPosition");
            }
        }

        public int FromLeft
        {
            get { if (_strategy.FromLeft) { return 0; } else { return 1; }; }
            set
            {
                _strategy.FromLeft = (value == 0);
                base.OnPropertyChanged("FromLeft");
            }
        }

        public int ToLeft
        {
            get { if (_strategy.ToLeft) { return 0; } else { return 1; }; }
            set
            {
                _strategy.ToLeft = (value == 0);
                base.OnPropertyChanged("ToLeft");
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

                if (propertyName == "FromPosition" || propertyName == "ToPosition")
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

            if (isNonNegative(_fromPos) && isNonNegative(_toPos))
                return null;

            return Resources.Position_Error_NotNonNegative;
        }

        #endregion // IDataErrorInfo Members

        private static bool isNonNegative(string text)
        {
            int num = 0;
            bool parse = int.TryParse(text, out num);
            return (parse && num >= 0);
        }
    }
}
