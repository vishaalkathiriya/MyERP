using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDAccidentRecord
    {
        public int SrNo { get; set; }
        public string TypeOfAccident { get; set; }
        public string Department { get; set; }
        public string ManagerName { get; set; }
        public string NameOfInjuredPerson { get; set; }
        public string RootCauseOfAccident { get; set; }
        public int NoOfCasualities { get; set; }
        public string CorrectiveActionTaken { get; set; }
        public bool Hospitalized { get; set; }
        public string NameOfHospital { get; set; }
        public Nullable<decimal> TreatmentExpenses { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
