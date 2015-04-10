﻿using System;
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
                StrategyTabs.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectFiles();
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
                    _bulkRenamer.RenameStrategy = new InsertTextStrategy(pos, InsertText.Text);
                    ICollectionView view = CollectionViewSource.GetDefaultView(FileNameListView.ItemsSource);
                    view.Refresh();
                }
            }
        }

        private void InsertStrategy_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetInsertStrategy();
        }

        private void InsertStrategy_TextChanged(object sender, TextCompositionEventArgs e)
        {
            SetInsertStrategy();
        }





    }
}
