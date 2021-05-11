using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers
{
    public class KYCController : Controller
    {
        //
        // GET: /KYC/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create(string id)
        {
            ViewBag.URLKey = id;
            ViewBag.TempImagePath = "../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString();

            ViewBag.mainDocPath = "../../" + ConfigurationManager.AppSettings["invClientUploads"].ToString();
            ViewBag.thumbDocPath = "../../" + ConfigurationManager.AppSettings["invClientThumbnails"].ToString();

            return View();
        }

        [HttpPost]
        public string UploadDocument(HttpPostedFileBase FileData)
        {
            try
            {
                //save main file
                string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(FileData.FileName));
                string mainPath = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);
                string thumbPath = Path.Combine(Server.MapPath("~/" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()), uniqueFilename);

                ViewBag.ImageThumbPath = "../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString();

                FileData.SaveAs(mainPath);

                //create thumbnail and save
                Bitmap bmpPostedImage = new System.Drawing.Bitmap(FileData.InputStream);
                Image objImage = ScaleImage(bmpPostedImage, 321, 200);
                objImage.Save(thumbPath, ImageFormat.Jpeg);

                return uniqueFilename;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
    }
}
