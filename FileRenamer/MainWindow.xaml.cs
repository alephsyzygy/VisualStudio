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
                FileNameListView.ItemsSource = _bulkRenamer.GetEnumerator();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectFiles();
        }



    }
}
