using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class RoleController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
