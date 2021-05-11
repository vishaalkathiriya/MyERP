using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class EmpRelativeInfoViewModel
    {
        public int EmployeeId { get; set; }
        public int SrNo { get; set; }
        public string RelativeName { get; set; }
        public int RelationId { get; set; }
        public string RelativeRelationName { get; set; }
        public string RelativeRelationNameOther { get; set; }
        public DateTime BirthDate { get; set; }
        public int AcedamicStatusId { get; set; }
        public string AcedamicStatus { get; set; }
        public int DegreeId { get; set; }
        public string DegreeName { get; set; }
        public string DegreeNameOther { get; set; }
        public string TypeOfWork { get; set; }
        public DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}