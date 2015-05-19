using FileRenamer.Properties;
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
    class DateInserterViewModel: StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        string _position;
        string[] _dateOptions;
        string[] _locations;
        DateInserterStrategy _strategy;

        #endregion

        #region Constructor

        public DateInserterViewModel()
        {
            _position = "0";
            _strategy = new DateInserterStrategy(DateTimeType.Current, 0, true, "yyyy/MM/dd HH:mm:ss");
            DisplayName = "Insert Date / Time";
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

        public string[] DateOptions
        {
            get
            {
                if (_dateOptions == null)
                {
                    _dateOptions = new string[] { "Current Date", "Date Created", "Date Modified", "Date Photo Taken" };
                }
                return _dateOptions;
            }
        }

        public string Position
        {
            get { return _strategy.Position.ToString(); }
            set
            {
                _position = value;

                int num = 0;
                bool parse = int.TryParse(_position, out num);
                _strategy.Position = num;

                base.OnPropertyChanged("Position");
            }
        }

        public string DateFormat
        {
            get { return _strategy.DateFormat; }
            set
            {
                _strategy.DateFormat = value;

                base.OnPropertyChanged("DateFormat");
            }
        }

        public int FromLeft
        {
            get { if (_strategy.FromLeft) {return 0;} else {return 1;}; }
            set
            {
                _strategy.FromLeft = (value == 0);
                base.OnPropertyChanged("FromLeft");
            }
        }

        public int DateType
        {
            get { return (int)_strategy.DateType; }
            set
            {
                if (value >= 0 && value < Enum.GetValues(typeof(DateTimeType)).Length)
                {
                    _strategy.DateType = (DateTimeType)value;
                    base.OnPropertyChanged("DateType");
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

            if (isNonNegative(_position))
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