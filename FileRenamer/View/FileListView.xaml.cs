using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileRenamer.View
{
    /// <summary>
    /// Interaction logic for FileListView.xaml
    /// </summary>
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();
        }
    }

    // Found online:
    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/80e09a3e-ddc0-4f37-aab8-743cceb364af/how-can-i-set-gridviewcolumns-width-as-relative-in-listview-in-wpf?forum=wpf
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int columnsCount = System.Convert.ToInt32(parameter);
            // Subtract some distance for scollbars.  There must be a better way of doing this.
            double width = (double)value - 1.6 * SystemParameters.VerticalScrollBarWidth;
            return width / columnsCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
