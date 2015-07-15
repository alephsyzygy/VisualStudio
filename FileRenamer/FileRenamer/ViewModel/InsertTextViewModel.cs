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
    class InsertTextViewModel : StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        string _position;
        string[] _locations;
        string[] _insertOptions;
        InsertTextStrategy _strategy;

        #endregion

        #region Constructor

        public InsertTextViewModel()
        {
            _position = "0";
            _strategy = new InsertTextStrategy(0, "", true, true);
            DisplayName = "Insert / Overwrite";
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

        public string[] InsertOptions
        {
            get
            {
                if (_insertOptions == null)
                {
                    _insertOptions = new string[] { "Insert", "Overwrite" };
                }
                return _insertOptions;
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

        public string Text
        {
            get { return _strategy.Text; }
            set
            {
                _strategy.Text = value;

                base.OnPropertyChanged("Text");
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

        public int InsertOrOverwrite
        {
            get { if (_strategy.InsertOrOverwrite) { return 0; } else { return 1; }; }
            set
            {
                _strategy.InsertOrOverwrite = (value == 0);
                base.OnPropertyChanged("InsertOrOverwrite");
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
