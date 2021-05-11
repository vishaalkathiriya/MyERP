using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblABGroup
    {
        public tblABGroup()
        {
            this.tblABGrp_Contact = new List<tblABGrp_Contact>();
        }

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupNote { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblABGrp_Contact> tblABGrp_Contact { get; set; }
    }
}
