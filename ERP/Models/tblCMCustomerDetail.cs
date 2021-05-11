using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblCMCustomerDetail
    {
        public int CustomerDetailId { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblCMCustomer tblCMCustomer { get; set; }
    }
}
