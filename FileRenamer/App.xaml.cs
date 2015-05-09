using FileRenamer.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FileRenamer.ViewModel;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow window = new MainWindow();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            //openFileDialog.Filter = "*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                //var fileMetaInfoList = openFileDialog.FileNames.Select(name => new FileMetaData(name)).ToList();
                var renamerModel = new MainWindowViewModel(openFileDialog.FileNames);
                //FileNameListView.ItemsSource = _bulkRenamer.Collection;
                //StrategyTabs.IsEnabled = true;

                window.DataContext = renamerModel;
                window.Show();
            }


        }
    }

}
