using ERP.ActionFilter;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers
{
    [HasLoginSessionFilter]
    [Audit]
    public class LogoutController : Controller
    {
        //
        // GET: /Logout/

        public ActionResult Index()
        {
            ERPUtilities.ToggleUserLoginStatus(Convert.ToInt32(Session["UserId"]), false);
            Session["UserId"] = null;
            Session["UserName"] = null;
            Session["ProfilePhoto"] = null;
            
            return RedirectToAction("Index", "Login");
        }

    }
}
