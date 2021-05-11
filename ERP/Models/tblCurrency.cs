using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblCurrency
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public short CountryId { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public virtual tblCountry tblCountry { get; set; }
    }
}
