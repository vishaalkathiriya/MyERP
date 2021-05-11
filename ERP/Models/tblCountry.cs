using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblCountry
    {
        public tblCountry()
        {
            this.tblCurrencies = new List<tblCurrency>();
            this.tblStates = new List<tblState>();
        }

        public short CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblCurrency> tblCurrencies { get; set; }
        public virtual ICollection<tblState> tblStates { get; set; }
    }
}
