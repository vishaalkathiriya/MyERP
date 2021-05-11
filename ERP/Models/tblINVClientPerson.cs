using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVClientPerson
    {
        public int PKId { get; set; }
        public int FKClientId { get; set; }
        public string Prefix { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public int IdentityDocId { get; set; }
        public string IdentityNo { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVClient tblINVClient { get; set; }
    }
}
