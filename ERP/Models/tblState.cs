using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblState
    {
        public short StateId { get; set; }
        public short CountryId { get; set; }
        public string StateName { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblCountry tblCountry { get; set; }
    }
}
