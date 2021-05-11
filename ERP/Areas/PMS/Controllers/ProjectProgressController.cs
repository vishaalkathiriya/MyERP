using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.PMS.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class ProjectProgressController : Controller
    {
        //
        // GET: /PMSProjectProgress/

        public ActionResult Index()
        {
            return View();
        }

    }
}
