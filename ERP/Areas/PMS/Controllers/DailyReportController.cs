using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.PMS.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class DailyReportController : Controller
    {
        //
        // GET: /PMSReport/

        public ActionResult Index()
        {
            //readMoreCounter
            ViewBag.readMoreCounter = ConfigurationManager.AppSettings["readMoreCounter"].ToString();
            return View();
        }

    }
}
