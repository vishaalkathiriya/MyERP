using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class EmpQualificationViewModel
    {
        public short SrNo { get; set; }
        public int EmployeeId { get; set; }
        public int Acedamic { get; set; }
        public string AcedamicName { get; set; }
        public int Degree { get; set; }
        public string DegreeName { get; set; }
        public string DegreeOther { get; set; }
        public int Discipline { get; set; }
        public string DisciplineName { get; set; }
        public string DisciplineOther { get; set; }
        public int University { get; set; }
        public string UniversityName { get; set; }
        public string UniversityOther { get; set; }
        public int Institute { get; set; }
        public string InstituteName { get; set; }
        public string InstituteOther { get; set; }
        public string PassingMonth { get; set; }
        public int PassingMonthDigit { get; set; }
        public int PassingYear { get; set; }
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime ChgDate { get; set; }
    }
}