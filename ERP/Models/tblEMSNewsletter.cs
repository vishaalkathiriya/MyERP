using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblEMSNewsletter
    {
        public int NewsletterID { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Too { get; set; }
        public string Subject { get; set; }
        public string HTML { get; set; }
        public string HeaderAndFooter { get; set; }
        public bool IsDeleted { get; set; }
        public int NrOpened { get; set; }
    }
}
