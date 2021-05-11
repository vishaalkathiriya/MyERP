using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRSubType
    {
        public tblSRSubType()
        {
            this.tblSRParameters = new List<tblSRParameter>();
        }

        public int SubTypeId { get; set; }
        public int TypeId { get; set; }
        public string SubTypeName { get; set; }
        public string Selection { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblSRParameter> tblSRParameters { get; set; }
        public virtual tblSRType tblSRType { get; set; }
    }
}
