using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class INVProjectViewModel
    {
        public int PKProjectId { get; set; }
        //public int FKInquiryId { get; set; }
        public string ProjectTitle { get; set; }
        //public int ProjectType { get; set; }
        public string ProjectType { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        //public System.DateTime StartDate { get; set; }
        //public System.DateTime EndDate { get; set; }
        //public int ProjectStatus { get; set; }
        public string ProjectStatus { get; set; }
        //public bool IsActive { get; set; }
        //public bool IsDeleted { get; set; }
        //public System.DateTime CreDate { get; set; }
        //public int CreBy { get; set; }
        //public System.DateTime ChgDate { get; set; }
        //public int ChgBy { get; set; }
        //public string Remarks { get; set; }
    }
}