using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Utilities;

namespace ERP.Controllers
{
    [HasLoginSessionFilter]
    [Audit]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.isTl = ERPUtilities.isTeamLeader(Convert.ToInt32(Session["UserId"]));
            ViewBag.isAdmin = Convert.ToInt32(Session["UserId"]);
            return View();
        }
    }
}
