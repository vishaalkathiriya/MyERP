using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class VisitorMaster
    {
        public int VisitorId { get; set; }
        public decimal MobileNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public Nullable<System.DateTime> PDate { get; set; }
        public string Remark { get; set; }
        public byte[] Photo { get; set; }
        public bool IsActive { get; set; }
    }
}
