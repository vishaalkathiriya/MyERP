using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Models;
using ERP.Utilities;
using System.Data;

namespace ERP.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {

            ERPContext context = new ERPContext();
            IList<tblDocument> listOfDocuments = context.tblDocuments.ToList();
            DataTable dt = listOfDocuments.ConvertToDataTable<tblDocument>();

            return View();
        }

    }
}
