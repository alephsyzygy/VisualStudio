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
using FileRenamer.IO;
using System.IO;
using Ninject;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool StartedFromExtension = false;
        private IEnumerable<string> FileNames;

        public void StartFromExtension(IEnumerable<string> FileNames)
        {
            StartedFromExtension = true;
            //this.FileNames = FileNames;
            //this.Run();

            MainWindowViewModel renamerModel = new MainWindowViewModel(FileNames.ToArray());
            MainWindow window = new MainWindow();
            window.DataContext = renamerModel;
            window.Show();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Kernel.kernel = new StandardKernel();
            AddBindings();

            string[] args = Environment.GetCommandLineArgs();

            using (TextWriter logger = File.CreateText("output.log"))
            {
                foreach (var arg in args)
                    logger.WriteLine("{0}", arg);
            }

            if (StartedFromExtension)
            {
                //MainWindowViewModel renamerModel = new MainWindowViewModel(FileNames.ToArray());
                //MainWindow window = new MainWindow();
                //window.DataContext = renamerModel;
                //window.Show();
            }
            else if (args.Count() > 1)
            {
                MainWindowViewModel renamerModel;
                // We are given some command line arguments, check to see if they are local or not
                if (args[1].Contains(":"))
                {
                    renamerModel = new MainWindowViewModel(args.Skip(1).ToArray());
                }
                else
                {
                    renamerModel =
                        new MainWindowViewModel(args.Skip(1)
                            .Select(file => Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + file)
                            .ToArray());
                }
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

        private void AddBindings()
        {
            Kernel.kernel.Bind<IIOMove>().To<IOMove>();
            //Kernel.kernel.Bind<IIOMove>().To<IOMoveLogger>();
        }
    }

    // For the moment we make it public, this should be changed in the future
    public static class Kernel
    {
        public static IKernel kernel;
    }

}
