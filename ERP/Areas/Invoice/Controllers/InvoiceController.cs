using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.ActionFilter;

namespace ERP.Areas.Invoice.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class InvoiceController : Controller
    {
        //
        // GET: /Invoice/Invoice/

        public ActionResult Index()
        {
            return View();
        }

    }
}
