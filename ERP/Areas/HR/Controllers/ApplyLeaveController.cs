using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.HR.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class ApplyLeaveController : Controller
    {
        public ActionResult Index(string id, string date)
        {
            ViewBag.EmployeeId= Request.QueryString["Id"] ?? "0";
            ViewBag.LeaveDate = Request.QueryString["Date"] ?? "'01-1990'";
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }
    }
}
