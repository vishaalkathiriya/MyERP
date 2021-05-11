using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.HR.Controllers
{
    [HasLoginSessionFilter]
    [Audit]
    public class ViewProfileController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.ProfileImage = "../../" + ConfigurationManager.AppSettings["UploadPath"].ToString();
            ViewBag.ProfileImageThumb = "../../" + ConfigurationManager.AppSettings["Thumbnails"].ToString();
            return View();
        }

        [HttpPost]
        public string employeeProfileImage(HttpPostedFileBase FileData)
        {
            try
            {
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string ProfileImage = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadPath"].ToString()), uniqueFilename);
                string ProfileImageThumb = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["Thumbnails"].ToString()), uniqueFilename);

                FileData.SaveAs(ProfileImage);
                FileData.SaveAs(ProfileImageThumb);
                return uniqueFilename;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
