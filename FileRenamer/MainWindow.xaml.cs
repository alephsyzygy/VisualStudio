using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.ComponentModel;
using System.Globalization;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BulkRenamer _bulkRenamer;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SelectFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            //openFileDialog.Filter = "*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                var fileMetaInfoList = openFileDialog.FileNames.Select(name => new FileMetaData(name)).ToList();
                _bulkRenamer = new BulkRenamer(fileMetaInfoList);
                FileNameListView.ItemsSource = _bulkRenamer.Collection;
                StrategyTabs.IsEnabled = true;
            }
        }

        private static bool isNonNegative(string text)
        {
            int num = 0;
            bool parse = int.TryParse(text, out num);
            return (parse && num >= 0);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isNonNegative(e.Text);
        }

        private void SetInsertStrategy()
        {
            if (_bulkRenamer != null)
            {
                int pos = 0;
                if (int.TryParse(InsertPos.Text, out pos))
                {
                    bool insertOrOverwrite = (cmbInsertOverwrite.SelectedIndex == 0);
                    bool fromLeft = cmbInsertPos.SelectedIndex == 0;
                    _bulkRenamer.RenameStrategy = new InsertTextStrategy(pos, InsertText.Text, fromLeft, insertOrOverwrite, (NameSuffixBehaviour) cmbNameSuffix.SelectedIndex);
                }
            }
        }

        private void SetRemoveStrategy()
        {
            if (_bulkRenamer != null)
            {
                int fromPos = 0;
                int toPos = 0;
                if (int.TryParse(txtFromPos.Text, out fromPos) && int.TryParse(txtToPos.Text, out toPos))
                {
                    bool fromLeft = (cmbFromPos.SelectedIndex == 0);
                    bool toLeft = (cmbToPos.SelectedIndex == 0);
                    _bulkRenamer.RenameStrategy = new RemoveCharactersStrategy(fromPos, fromLeft, toPos, toLeft, (NameSuffixBehaviour) cmbNameSuffix.SelectedIndex);
                }
            }
        }

        private void SetNumberingStrategy()
        {
            if (_bulkRenamer != null)
            {
                _bulkRenamer.RenameStrategy = new NumberingStrategy((NumberingFormat)cmbNumberFormat.SelectedIndex, (NumberingTextFormat)cmbTextFormat.SelectedIndex,
                                                                    txtStartWith.Text, txtNumberText.Text, (NameSuffixBehaviour)cmbNameSuffix.SelectedIndex);
            }
        }

        private void SetDateStrategy()
        {
            if (_bulkRenamer != null)
            {
                int pos = 0;
                if (int.TryParse(txtDatePos.Text, out pos))
                {
                    bool fromLeft = (cmbDatePos.SelectedIndex == 0);
                    _bulkRenamer.RenameStrategy = new DateInserterStrategy((DateTimeType)cmbInsertTime.SelectedIndex, pos, fromLeft, txtDateFormat.Text, (NameSuffixBehaviour)cmbNameSuffix.SelectedIndex);
                }
            }
        }

        private void SetCaseChangeStrategy()
        {
            if (_bulkRenamer != null)
            {
                _bulkRenamer.RenameStrategy = new CaseChangerStrategy((CaseTypes)cmbConvertTo.SelectedIndex, (NameSuffixBehaviour)cmbNameSuffix.SelectedIndex);
            }
        }

        private void NumberingStrategy_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetNumberingStrategy();
        }

        private void NumberingStrategy_ComboChanged(object sender, SelectionChangedEventArgs e)
        {
            SetNumberingStrategy();
        }

        private void InsertStrategy_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetInsertStrategy();
        }

        private void InsertStrategy_TextChanged(object sender, TextCompositionEventArgs e)
        {
            SetInsertStrategy();
        }

        private void btnChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            SelectFiles();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void cmbInsertOverwrite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (StrategyTabs.SelectedIndex)
            {
                case 0:
                    SetInsertStrategy();
                    break;
                case 1:
                    SetNumberingStrategy();
                    break;
                case 2:
                    SetRemoveStrategy();
                    break;
                case 3:
                    break;
                case 4:
                    SetCaseChangeStrategy();
                    break;
                case 5:
                    SetDateStrategy();
                    break;
                default:
                    break;
            }
        }

        private void RemoveStrategy_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetRemoveStrategy();
        }

        private void RemoveStrategy_ComboChanged(object sender, SelectionChangedEventArgs e)
        {
            SetRemoveStrategy();
        }

        private void CaseChangeStrategy_ComboChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCaseChangeStrategy();
        }

        private void DateStrategy_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetDateStrategy();
        }

        private void DateStrategy_ComboChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDateStrategy();
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
