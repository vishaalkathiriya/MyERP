using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.AB.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class PrintAddressController : Controller
    {
        //
        // GET: /ABPrintAddress/

        public ActionResult Index()
        {
            return View();
        }

    }
}
