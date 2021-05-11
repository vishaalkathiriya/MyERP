using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class ABContactViewModel
    {
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
        public string GroupName { set; get; }
        public string AreaCity { set; get; }
    }
}