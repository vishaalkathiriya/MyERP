using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class HasAccessViewModel
    {
        public bool HasInsertRights { get; set; }
        public bool HasUpdateRights { get; set; }
        public bool HasDeleteRights { get; set; }
    }
}