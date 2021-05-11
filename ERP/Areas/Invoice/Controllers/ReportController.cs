using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.Invoice.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class ReportController : Controller
    {
        //
        // GET: /Invoice/Report/
        public ActionResult Index()
        {
            return View();
        }

    }
}
