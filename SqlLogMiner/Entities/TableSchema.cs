using System.Collections.Generic;

namespace SqlLogMiner.Entities
{
    public class TableSchema
    {
        public List<Column> Columns { get; set; }

        public TableSchema()
        {
            Columns = new List<Column>();
        }
    }
}