using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class EmpCredentialViewModel
    {
        public short SrNo { get; set; }
        public int EmployeeId { get; set; }
        public short SourceId { get; set; }
        public string SourceOther { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string EmailId { get; set; }
        public string SecurityQuestion1 { get; set; }
        public string SecurityAnswer1 { get; set; }
        public string SecurityQuestion2 { get; set; }
        public string SecurityAnswer2 { get; set; }
        public string Comments { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}