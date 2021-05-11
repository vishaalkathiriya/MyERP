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
    public class TrainingsAndMeetingController : Controller
    {
        //
        // GET: /HRDTrainingsAndMeeting/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.tempDocPdf = "../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString();
            ViewBag.DocPdf = "../../" + ConfigurationManager.AppSettings["UploadTrainingAndMeeting"].ToString();
            return View();
        }

        [HttpPost]
        public string traingMeetingDocument(HttpPostedFileBase FileData)
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
