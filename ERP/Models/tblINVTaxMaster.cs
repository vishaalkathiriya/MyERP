using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVTaxMaster
    {
        public tblINVTaxMaster()
        {
            this.tblINVInvoiceTaxes = new List<tblINVInvoiceTax>();
        }

        public int PKTaxId { get; set; }
        public string TaxTypeName { get; set; }
        public string Mode { get; set; }
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblINVInvoiceTax> tblINVInvoiceTaxes { get; set; }
    }
}
