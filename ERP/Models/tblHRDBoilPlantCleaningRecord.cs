using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDBoilPlantCleaningRecord
    {
        public int SrNo { get; set; }
        public string BoilPlantLocation { get; set; }
        public System.DateTime DateOfCleaining { get; set; }
        public string NameOfCleaner { get; set; }
        public string PlantIncharge { get; set; }
        public string Remark { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
