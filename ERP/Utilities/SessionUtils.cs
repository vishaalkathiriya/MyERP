using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Utilities
{
    public class SessionUtils
    {
        public bool HasUserLogin()
        {
            return HttpContext.Current.Session["UserId"] != null;
        }
    }
}