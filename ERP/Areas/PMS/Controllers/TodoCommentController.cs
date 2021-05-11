using ERP.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Areas.PMS.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class TodoCommentController : Controller
    {
        //
        // GET: /PMSTodoComment/

        public ActionResult Index(int id)
        {
            if (Request.UrlReferrer == null) {
                Response.Redirect("/PMS/PMSProject");
            }

            ViewBag.ID = id;
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.mainPath = ConfigurationManager.AppSettings["pmsUploads"].ToString();
            ViewBag.thumbPath = ConfigurationManager.AppSettings["pmsThumbnails"].ToString();
            ViewBag.profilePath = ConfigurationManager.AppSettings["UploadPath"].ToString(); //for employee profile pix
            return View();
        }

        [HttpPost]
        public string UploadDocument(HttpPostedFileBase FileData)
        {
            try {
                //save main file
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string tempMainPath = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);
                string tempThumbPath = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()), uniqueFilename);

                FileData.SaveAs(tempMainPath);

                string ext = Path.GetExtension(FileData.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png") {
                    //create thumbnail and save
                    Bitmap bmpPostedImage = new System.Drawing.Bitmap(FileData.InputStream);
                    Image objImage = ScaleImage(bmpPostedImage, 250, 150);
                    if (ext == ".jpg" || ext == ".jpeg") {
                        objImage.Save(tempThumbPath, ImageFormat.Jpeg);
                    }
                    else if (ext == ".png") {
                        objImage.Save(tempThumbPath, ImageFormat.Png);
                    }
                }

                return uniqueFilename;
            }
            catch {
                return string.Empty;
            }
        }

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var newImage = new Bitmap(maxWidth, maxHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, maxWidth, maxHeight);
            }
            return newImage;
        }
    }
}
