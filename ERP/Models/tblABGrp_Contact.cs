using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblABGrp_Contact
    {
        public int Grp_ContactId { get; set; }
        public int ContactId { get; set; }
        public int GroupId { get; set; }
        public virtual tblABContact tblABContact { get; set; }
        public virtual tblABGroup tblABGroup { get; set; }
    }
}
