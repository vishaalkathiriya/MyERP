using ERP.ActionFilter;
using ERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.AB.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class ContactController : Controller
    {
        //
        // GET: /ABContact/

        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.TempImageThumbPath = "../../" + ConfigurationManager.AppSettings["TempABContactExcelPath"].ToString();
            ViewBag.mainDocPath = "../../" + ConfigurationManager.AppSettings["UploadContactPath"].ToString();
            return View();
        }

        [HttpPost]
        public string UploadExcel(HttpPostedFileBase FileData)
        {
            try
            {
                //save main file
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string mainPath = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["TempABContactExcelPath"].ToString()), uniqueFilename);

                ViewBag.ImageThumbPath = "../../" + ConfigurationManager.AppSettings["TempABContactExcelPath"].ToString();
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
