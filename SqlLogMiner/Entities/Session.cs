﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlLogMiner.Entities
{
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
        public string Tables { get; set; }
    }
}