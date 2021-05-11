using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVLogInquiry
    {
        public int PKLogId { get; set; }
        public string Description { get; set; }
        public int CreBy { get; set; }
        public System.DateTime CreDate { get; set; }
    }
}
