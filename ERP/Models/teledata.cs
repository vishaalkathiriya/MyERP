using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class teledata
    {
        public decimal SrNo { get; set; }
        public string Type { get; set; }
        public System.DateTime PDate { get; set; }
        public string Outline { get; set; }
        public decimal Duration { get; set; }
        public string Ext { get; set; }
        public string OutNumber { get; set; }
        public string Ringing { get; set; }
    }
}
