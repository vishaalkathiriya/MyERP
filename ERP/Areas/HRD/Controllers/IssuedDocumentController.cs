using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.HRD.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class IssuedDocumentController : Controller
    {
        //
        // GET: /IssuedDocument/

        public ActionResult Index()
        {
            ViewBag.ImagePath ="../../" + ConfigurationManager.AppSettings["UploadIssuedDocument"].ToString();
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

        [HttpPost]
        public string UploadIssuedDoc(HttpPostedFileBase FileData)
        {
            try
            {
                //save main file
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string mainPath = Path.Combine(Server.MapPath("../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);

                FileData.SaveAs(mainPath);

                return uniqueFilename;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
