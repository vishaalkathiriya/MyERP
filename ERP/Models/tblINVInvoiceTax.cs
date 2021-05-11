using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVInvoiceTax
    {
        public int PKInvoiceTaxId { get; set; }
        public int FKInvoiceId { get; set; }
        public int FKTaxId { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVInvoice tblINVInvoice { get; set; }
        public virtual tblINVTaxMaster tblINVTaxMaster { get; set; }
    }
}
