using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEMSMailSent
    {
        public int MailID { get; set; }
        public int NewsletterID { get; set; }
        public int ClientID { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public System.DateTime DatePickedup { get; set; }
        public System.DateTime DateOpened { get; set; }
    }
}
