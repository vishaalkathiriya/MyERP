using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class InOutEntry
    {
        public int ECode { get; set; }
        public int SrNo { get; set; }
        public Nullable<int> Machine { get; set; }
        public Nullable<System.DateTime> InOutTime { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
