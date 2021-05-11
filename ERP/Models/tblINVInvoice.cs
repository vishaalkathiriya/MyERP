using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVInvoice
    {
        public tblINVInvoice()
        {
            this.tblINVInvoiceTaxes = new List<tblINVInvoiceTax>();
            this.tblINVPayments = new List<tblINVPayment>();
        }

        public int PKInvoiceId { get; set; }
        public int FKClientId { get; set; }
        public string MilestoneIds { get; set; }
        public string InvoiceCode { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public string Currency { get; set; }
        public string Remarks { get; set; }
        public decimal RoundOff { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVClient tblINVClient { get; set; }
        public virtual ICollection<tblINVInvoiceTax> tblINVInvoiceTaxes { get; set; }
        public virtual ICollection<tblINVPayment> tblINVPayments { get; set; }
    }
}
