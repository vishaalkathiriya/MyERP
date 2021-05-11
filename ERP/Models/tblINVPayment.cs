using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVPayment
    {
        public int PKPaymentId { get; set; }
        public int FKInvoiceId { get; set; }
        public System.DateTime PaymentReceivedDate { get; set; }
        public decimal OnHandReceivedAmount { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal ExchangeRateINR { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVInvoice tblINVInvoice { get; set; }
    }
}
