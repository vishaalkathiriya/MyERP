using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PaymentInvoiceListViewModel
    {
        public int PKInvoiceId { get; set; }
        public decimal PaymentReceivedAmount { get; set; }
        public string InvoiceCode { get; set; }
        public string Currency { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}