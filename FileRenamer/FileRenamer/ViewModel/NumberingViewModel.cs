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
    class NumberingViewModel : StrategyViewModel, IDataErrorInfo
    {
        #region Fields

        string[] _numberingFormats;
        string[] _textFormats;
        NumberingStrategy _strategy;

        #endregion

        #region Constructor

        public NumberingViewModel()
        {
            _strategy = new NumberingStrategy(NumberingFormat.NoZeros,NumberingTextFormat.OldNameTextNumber,"1","");
            DisplayName = "Numbering";
        }

        #endregion

        #region Properties

        override public IFileRenamerStrategy Strategy
        {
            get { return _strategy; }
        }

        public string[] NumberingFormats
        {
            get
            {
                if (_numberingFormats == null)
                {
                    _numberingFormats = new string[] {"1, 2, 3, ...", "01, 02, 03, ...", "001, 002, 003, ...", "0001, 0002, 0003, ...", "a, b, c, ..." };
                }
                return _numberingFormats;
            }
        }

        public string[] TextFormats
        {
            get
            {
                if (_textFormats == null)
                {
                    _textFormats = new string[] {"Old Name - Text - Number","Number - Text - Old Name","Text - Number" ,"Number - Text" };
                }
                return _textFormats;
            }
        }


        public string Start
        {
            get { return _strategy.Start; }
            set
            {
                _strategy.Start = value;

                base.OnPropertyChanged("Start");
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

        public int NumberFormat
        {
            get { return (int)_strategy.NumberFormat; }
            set
            {
                if (value >= 0 && value < Enum.GetNames(typeof(NumberingFormat)).Length)
                {
                    _strategy.NumberFormat = (NumberingFormat)value;

                    base.OnPropertyChanged("NumberFormat");
                }
            }

        }

        public int TextFormat
        {
            get { return (int)_strategy.TextFormat; }
            set
            {
                if (value >= 0 && value < Enum.GetNames(typeof(NumberingTextFormat)).Length)
                {
                    _strategy.TextFormat = (NumberingTextFormat)value;

                    base.OnPropertyChanged("TextFormat");
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
                return null;
        }

        #endregion // IDataErrorInfo Members

}
}
