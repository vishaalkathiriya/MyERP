using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblVendor
    {
        public tblVendor()
        {
            this.tblInvetories = new List<tblInvetory>();
        }

        public short VendorId { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Mobile { get; set; }
        public string PhoneNo { get; set; }
        public string Services { get; set; }
        public short Rating { get; set; }
        public string HouseNo { get; set; }
        public string Location { get; set; }
        public string Area { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public short CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public short ChgBy { get; set; }
        public virtual ICollection<tblInvetory> tblInvetories { get; set; }
    }
}
