using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlLogMiner.Entities
{
    [Serializable]
    public class Session
    {
        public string ServerName { get; set; }
        public string Authentication { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string LogPath { get; set; }
        public bool WholeLogSearch { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool InsertOperation { get; set; }
        public bool DeleteOperation { get; set; }
        public bool UpdateOperation { get; set; }
        public List<string> Tables { get; set; }

        public Session()
        {
            From = DateTime.UtcNow;
            To = DateTime.UtcNow;
            InsertOperation = true;
            DeleteOperation = true;
            UpdateOperation = true;
            WholeLogSearch = true;
            Tables = new List<string>();
        }

        public string[] GetOperationsList()
        {
            List<string> operations = new List<string>();

            if(InsertOperation)
                operations.Add("LOP_INSERT_ROWS");
            if(DeleteOperation)
                operations.Add("LOP_DELETE_ROWS");
            if(UpdateOperation)
                operations.Add("LOP_UPDATE_ROWS");

            return operations.ToArray();
        }
    }
}
