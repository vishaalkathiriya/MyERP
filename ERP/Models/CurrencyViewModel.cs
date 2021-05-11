using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public short CountryId { get; set; }
        public string  CountryName { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
    }
}