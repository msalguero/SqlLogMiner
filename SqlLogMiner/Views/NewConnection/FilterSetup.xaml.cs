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
using SqlLogMiner.Entities;

namespace SqlLogMiner.Views.NewConnection
{
    /// <summary>
    /// Interaction logic for FilterSetup.xaml
    /// </summary>
    public partial class FilterSetup : Window
    {
        public Session NewSession;
        public FilterSetup(Session newSession)
        {
            InitializeComponent();
            NewSession = newSession;
            DataContext = NewSession;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            Close();
            LogSelection logSelectionWindow = new LogSelection(NewSession);
            logSelectionWindow.ShowDialog();
        }

        private void Finish(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }

   
}
