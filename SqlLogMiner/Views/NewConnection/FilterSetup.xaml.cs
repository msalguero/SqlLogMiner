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
using SqlLogMiner.Models;

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
            UserTableDataGrid.ItemsSource = SqlServerManager.GetUserTables(NewSession.ServerName,NewSession.Authentication == "Windows Authentication",NewSession.UserName,NewSession.Password,NewSession.Database);
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
            foreach (DataGridFilterTableModel row in UserTableDataGrid.ItemsSource)
            {
                if (row.IsChecked)
                {
                    NewSession.Tables.Add(row.SchemaName + "." + row.TableName);
                }
            }
            DialogResult = true;
        }
    }

   
}
