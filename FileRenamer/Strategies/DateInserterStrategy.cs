using FileRenamer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRenamer.Strategies
{
    public enum DateTimeType
    {
        Current,
        Created,
        Modified,
        PhotoTaken
    }

    public class DateInserterStrategy : IFileRenamerStrategy
    {
        private DateTimeType _dateType;
        private int _position;
        private bool _fromLeft;
        private string _dateFormat;

        public int Position { get { return _position; } set { _position = value; OnChanged(EventArgs.Empty); } }
        public DateTimeType DateType { get { return _dateType; } set { _dateType = value; OnChanged(EventArgs.Empty); } }
        public bool FromLeft { get { return _fromLeft; } set { _fromLeft = value; OnChanged(EventArgs.Empty); } }
        public string DateFormat { get { return _dateFormat; } set { _dateFormat = value; OnChanged(EventArgs.Empty); } }

        public DateInserterStrategy(DateTimeType DateType, int Position, bool FromLeft, string DateFormat)
        {
            _dateType = DateType;
            _position = Position;
            _fromLeft = FromLeft;
            _dateFormat = DateFormat;
        }

        public string RenameFile(FileMetaData FileName, int Position, NameExtensionHelper Helper)
        {
            Helper.Text = FileName.Name;
            DateTime date;
            switch (_dateType)
            {
                case DateTimeType.Current:
                    date = DateTime.Now;
                    break;
                case DateTimeType.Created:
                    date = FileName.Created;
                    break;
                case DateTimeType.Modified:
                    date = FileName.Modified;
                    break;
                case DateTimeType.PhotoTaken:
                    goto default;
                default:
                    date = DateTime.Now;
                    break;
            }
            string dateString;
            try
            {
                dateString = String.Format("{0:" + _dateFormat + "}", date);
            }
            catch (FormatException)
            {
                dateString = "";
            }
            return Helper.Insert(_position, _fromLeft, dateString);
        }

        #region Events

        public event EventHandler<EventArgs> StrategyChanged;

        protected void OnChanged(EventArgs e)
        {
            if (StrategyChanged != null)
                StrategyChanged(this, e);
        }

        #endregion
    }
}
