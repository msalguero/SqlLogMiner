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

        private void New(object sender, RoutedEventArgs e)
        {
            ConnectDatabase connectDatabase = new ConnectDatabase();
            
            if (connectDatabase.ShowDialog() == true)
            {
                CurrentSession = connectDatabase.NewSession;
            }
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentSession = FileManager.DeSerializeObject<Session>(openFileDialog.FileName);
            }
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
    }
}
