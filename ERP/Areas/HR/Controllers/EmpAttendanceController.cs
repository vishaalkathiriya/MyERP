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
    public class EmpAttendanceController : Controller
    {
        //
        // GET: /HR/EmpAttendance/
        
        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
