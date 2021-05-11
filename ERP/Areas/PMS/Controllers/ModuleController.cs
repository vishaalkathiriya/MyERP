using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace ERP.Areas.PMS.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class ModuleController : Controller
    {
        //
        // GET: /PMSModule/

        public ActionResult Index(int id)
        {
            if (Request.UrlReferrer == null) {
                Response.Redirect("/PMS/PMSProject");
            }

            ViewBag.ID = id;
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
