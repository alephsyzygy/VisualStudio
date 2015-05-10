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
using System.Windows.Shapes;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public int Result
        {
            get;
            set;
        }
        public ErrorDialog()
        {
            InitializeComponent();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            Result = 1;
            this.DialogResult = true;
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            Result = 2;
            this.DialogResult = true;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            Result = 3;
            this.DialogResult = true;
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            Result = 4;
            this.DialogResult = true;
        }
    }
}
