using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlLogMiner.Entities
{
    public class TransactionLogRow
    {
        public string TransactionId { get;set; }
        public string Operation { get; set; }
        public string Schema { get; set; }
        public string Object { get; set; }
        public string User { get; set; }
        public DateTime BeginTime { get; set; }
        public string LSN { get; set; }
        public byte[] RowLogContents0 { get; set; }
        public byte[] RowLogContents1 { get; set; }
        public bool RowChecked { get; set; }
    }
}
