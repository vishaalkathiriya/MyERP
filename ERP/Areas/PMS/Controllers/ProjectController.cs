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
    public class ProjectController : Controller
    {
        //
        // GET: /PMSProject/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

        public JavaScriptResult GetProjectTypes()
        {
            var types = ConfigurationManager.AppSettings.Get("ProjectType").ToString();
            var arrTypes = types.Split(',');
            var jScript = "var listOfProjectTypes = {";
            foreach (var type in arrTypes)
            {
                var keyValue = type.Split(':');
                jScript += "'"+ keyValue[1] +"':'"+ keyValue[0] +"',";
            }
            jScript += "};";
            return JavaScript(jScript);
        }

    }
}
