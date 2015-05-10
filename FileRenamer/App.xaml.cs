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
using System.IO;

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

            string[] args = Environment.GetCommandLineArgs();

            if (args.Count() > 1)
            {
                // We are given some command line arguments
                MainWindowViewModel renamerModel = new MainWindowViewModel(args.Skip(1)
                                                                               .Select(file => Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + file)
                                                                               .ToArray());
                MainWindow window = new MainWindow();
                window.DataContext = renamerModel;
                window.Show();
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                //openFileDialog.Filter = "*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    MainWindowViewModel renamerModel = new MainWindowViewModel(openFileDialog.FileNames);
                    MainWindow window = new MainWindow();
                    window.DataContext = renamerModel;
                    window.Show();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }


        }
    }

}
