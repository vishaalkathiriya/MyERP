using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers
{
    [HasLoginSessionFilter]
    [Audit]
    public class StaticPagesController : Controller
    {
        //
        // GET: /StaticPages/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InventoryDetail()
        {
            return View();
        }

    }
}
