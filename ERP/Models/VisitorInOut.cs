using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class VisitorInOut
    {
        public int SrNo { get; set; }
        public int VisitorId { get; set; }
        public int ECode { get; set; }
        public string RefName { get; set; }
        public string Department { get; set; }
        public string Manager { get; set; }
        public decimal ExtNo { get; set; }
        public System.DateTime InTime { get; set; }
        public Nullable<System.DateTime> OutTime { get; set; }
        public string Reason { get; set; }
        public Nullable<int> Person { get; set; }
    }
}
