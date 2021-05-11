﻿using ERP.ActionFilter;
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
    public class TechnologyController : Controller
    {
        //
        // GET: /TechnologyMaster/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

    }
}
