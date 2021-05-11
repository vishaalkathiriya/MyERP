using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVClientSource
    {
        public int PKSourceId { get; set; }
        public string SourceName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
