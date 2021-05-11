using ERP.ActionFilter;
using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.HR.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class OrgChartController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
