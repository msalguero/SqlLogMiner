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
using Microsoft.Win32;
using SqlLogMiner.Entities;

namespace SqlLogMiner.Views.NewConnection
{
    /// <summary>
    /// Interaction logic for LogSelection.xaml
    /// </summary>
    public partial class LogSelection : Window
    {
        public Session NewSession;
        public LogSelection(Session newSession)
        {
            InitializeComponent();
            NewSession = newSession;
            DataContext = NewSession;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                LogsListBox.Items.Add(openFileDialog.SafeFileName);
            }
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            this.Close();
            ConnectDatabase connectDatabase = new ConnectDatabase();
            connectDatabase.ShowDialog();
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            FilterSetup filterSetupWindow = new FilterSetup(NewSession);
            if (filterSetupWindow.ShowDialog() == true)
            {
                DialogResult = true;
            }
        }
    }
}
