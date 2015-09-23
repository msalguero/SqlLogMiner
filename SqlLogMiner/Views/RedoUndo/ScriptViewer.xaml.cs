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

namespace SqlLogMiner.Views.RedoUndo
{
    /// <summary>
    /// Interaction logic for ScriptViewer.xaml
    /// </summary>
    public partial class ScriptViewer : Window
    {
        public string ScriptText;
        public ScriptViewer(string scriptText)
        {
            InitializeComponent();
            ScriptText = scriptText;
            ScriptTextBox.Text = ScriptText;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
