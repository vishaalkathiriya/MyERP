using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.HRD.Controllers
{
    public class ChemicalStorageInspactionLogController : Controller
    {

        //
        // GET: /HRD/ChemicalStorageInspactionLog/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.TempDocPdf = "../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString();
            ViewBag.DocPdf = "../../" + ConfigurationManager.AppSettings["UploadChemicalStorageInspactionLog"].ToString();
            return View();
        }
        [HttpPost]
        public string document(HttpPostedFile FileData)
        {
            try
            {
                //Save Main File
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string mainpath = Path.Combine(Server.MapPath("../" + ConfigurationManager.AppSettings["UploadChemicalStorageInspactionLog"].ToString()), uniqueFilename);
                FileData.SaveAs(mainpath);
                return uniqueFilename;
            }
            catch
            {
                return string.Empty;
            }

        }

    }
}
