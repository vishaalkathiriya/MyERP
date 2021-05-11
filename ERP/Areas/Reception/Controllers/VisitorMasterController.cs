using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.Reception.Controllers
{
    [HasLoginSessionFilter]
    [Audit]
    public class VisitorMasterController : Controller
    {
        //
        // GET: /Reception/EmployeeDetail/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
