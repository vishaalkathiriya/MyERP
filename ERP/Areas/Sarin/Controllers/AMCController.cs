using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.ActionFilter;

namespace ERP.Areas.Sarin.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class AMCController : Controller
    {
        //
        // GET: /Sarin/AMC/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
