using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEMSGroupClient
    {
        public int RecordID { get; set; }
        public int ClientGroupID { get; set; }
        public int ClientID { get; set; }
        public bool IsDeleted { get; set; }
    }
}
