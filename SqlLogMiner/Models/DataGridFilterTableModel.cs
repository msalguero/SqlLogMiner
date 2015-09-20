using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlLogMiner.Models
{
    public class DataGridFilterTableModel
    {
        public bool IsChecked { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}
