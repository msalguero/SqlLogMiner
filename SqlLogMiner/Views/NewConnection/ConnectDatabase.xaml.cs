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
    /// Interaction logic for ConnectDatabase.xaml
    /// </summary>
    public partial class ConnectDatabase : Window
    {
        public Session NewSession;
        public ConnectDatabase()
        {
            InitializeComponent();

            NewSession = new Session();
            DataContext = NewSession;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            LogSelection logSelectionWindow = new LogSelection(NewSession);
            if (logSelectionWindow.ShowDialog() == true)
            {
                DialogResult = true;
            }
            
        }
    }
}
