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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SqlLogMiner.Entities;
using SqlLogMiner.Views.NewConnection;

namespace SqlLogMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string SavePath { get; set; }
        public Session CurrentSession { get; set; }
        public SqlServerManager SqlServerManager { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            SavePath = "";
            SqlServerManager = new SqlServerManager();
        }

        private void ConnectToDatabase()
        {
            if (CurrentSession.Authentication == "Windows Authentication")
                SqlServerManager.ConnectWindowsAuth(CurrentSession.ServerName, CurrentSession.Database);
            else
                SqlServerManager.ConnectSqlServerAuth(CurrentSession.ServerName, CurrentSession.Database,
                    CurrentSession.UserName, CurrentSession.Password);

            GetTransactionLog();
        }

        private void GetTransactionLog()
        {

            TransactionLogGrid.ItemsSource = SqlServerManager.GetTransactionLog(CurrentSession.From, CurrentSession.To, CurrentSession.GetOperationsList(), CurrentSession.Tables.ToArray());
        }

        private void New(object sender, RoutedEventArgs e)
        {
            if (CurrentSession != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Save changes to session?", "New Session",MessageBoxButton.YesNoCancel);
                    if (messageBoxResult == MessageBoxResult.Yes)
                        Save(sender,e);
                if (messageBoxResult == MessageBoxResult.Cancel)
                    return;
                SqlServerManager.Disconnect();
            }
            ConnectDatabase connectDatabase = new ConnectDatabase();
            
            if (connectDatabase.ShowDialog() == true)
            {
                CurrentSession = connectDatabase.NewSession;
                ConnectToDatabase();
            }
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentSession = FileManager.DeSerializeObject<Session>(openFileDialog.FileName);
                ConnectToDatabase();
            }
            GetTransactionLog();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if(SavePath == "")
                SaveAs(sender, e);
            else
            {
                FileManager.SerializeObject(CurrentSession, SavePath);
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                SavePath = saveFileDialog.FileName;
                FileManager.SerializeObject<Session>(CurrentSession, SavePath);
            }
        }

        private void OnSelectedRowChanged(object sender, SelectionChangedEventArgs e)
        {
            TransactionLogRow selectedRow = (TransactionLogRow) TransactionLogGrid.SelectedItem;
            TableSchema selectedTableSchema = SqlServerManager.GetTableSchema(CurrentSession.Database, selectedRow.Object);
            TransactionLogInterpreter.InterpretRowLogContent(selectedRow.RowLogContents0,ref selectedTableSchema);
            RowDetailsGrid.ItemsSource = selectedTableSchema.Columns;
        }
    }
}
