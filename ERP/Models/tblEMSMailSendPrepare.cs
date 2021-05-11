using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEMSMailSendPrepare
    {
        public int RecordID { get; set; }
        public int NewsletterID { get; set; }
        public int ClientGroupID { get; set; }
        public int ClientID { get; set; }
        public System.DateTime DatePickup { get; set; }
        public bool IsDeleted { get; set; }
    }
}
