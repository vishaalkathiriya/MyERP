using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblSRParameter
    {
        public int ParameterId { get; set; }
        public int SubTypeId { get; set; }
        public string ParameterName { get; set; }
        public string Remarks { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblSRSubType tblSRSubType { get; set; }
    }
}
