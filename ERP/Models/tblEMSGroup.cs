using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEMSGroup
    {
        public int ClientGroupID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreDate { get; set; }
    }
}
