using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblCMCustomer
    {
        public tblCMCustomer()
        {
            this.tblCMCustomerDetails = new List<tblCMCustomerDetail>();
        }

        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string BRN { get; set; }
        public Nullable<decimal> VAT { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblCMCustomerDetail> tblCMCustomerDetails { get; set; }
    }
}
