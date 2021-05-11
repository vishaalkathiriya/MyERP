using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ERP.ActionFilter;

namespace ERP.Areas.PMS.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class ModuleTodoController : Controller
    {
        //
        // GET: /PMSModuleTodo/

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
