using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlLogMiner.Entities
{
    public class Column
    {
        public string ColumnName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
