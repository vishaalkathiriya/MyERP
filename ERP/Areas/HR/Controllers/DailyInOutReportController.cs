using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.HR.Controllers
{
    [HasLoginSessionFilter]
    [Audit]
    public class DailyInOutReportController : Controller
    {
        //
        // GET: /PMS/DailyInOutReport/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
