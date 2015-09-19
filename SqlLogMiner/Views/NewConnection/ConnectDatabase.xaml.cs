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
        public List<string> ServerInstances { get; set; } 
        public ConnectDatabase()
        {
            InitializeComponent();

            NewSession = new Session();
            ServerInstances = SqlServerManager.GetSqlServerInstances();
            DataContext = NewSession;
            ServerListBox.ItemsSource = ServerInstances;
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

        private void OnDropDownOpen(object sender, EventArgs e)
        {
            try
            {
                DatabaseTextBox.ItemsSource = SqlServerManager.GetDatabases(NewSession.ServerName, NewSession.Authentication == "Windows Authentication",
                NewSession.UserName, NewSession.Password);
            }
            catch (Exception)
            {

                MessageBox.Show("Could not Connect");
            }
            
        }
    }
}
