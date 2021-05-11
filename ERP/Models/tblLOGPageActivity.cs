using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblLOGPageActivity
    {
        public int LogId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string IPAddress { get; set; }
        public string Url { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
    }
}
