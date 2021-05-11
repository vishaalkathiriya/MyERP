using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRType
    {
        public tblSRType()
        {
            this.tblSRSubTypes = new List<tblSRSubType>();
        }

        public int TypeId { get; set; }
        public string TypePrefix { get; set; }
        public string TypeName { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblSRSubType> tblSRSubTypes { get; set; }
    }
}
