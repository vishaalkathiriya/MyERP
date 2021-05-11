using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblABContact
    {
        public tblABContact()
        {
            this.tblABGrp_Contact = new List<tblABGrp_Contact>();
        }

        public int ContactId { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string LandlineNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string CompanyName { get; set; }
        public string Note { get; set; }
        public int LangId { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblABLanguage tblABLanguage { get; set; }
        public virtual ICollection<tblABGrp_Contact> tblABGrp_Contact { get; set; }
    }
}
