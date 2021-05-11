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

namespace ERP.Areas.Invoice.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class InquiryController : Controller
    {
        //
        // GET: /Invoice/Inquiry/
       
        public ActionResult Index(int? id)
        {
            ViewBag.ClientId = id;
            if (Request.QueryString["cid"] != null)
            {
                ViewBag.InquiryId = Request.QueryString["cid"].ToString();
            }
            if (Request.QueryString["action"] != null)
            {
                ViewBag.Action = Request.QueryString["action"].ToString();
            }
            
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

                FileData.SaveAs(mainPath);
                string ext = Path.GetExtension(FileData.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    //create thumbnail and save
                    Bitmap bmpPostedImage = new System.Drawing.Bitmap(FileData.InputStream);
                    Image objImage = ScaleImage(bmpPostedImage, 250, 150);
                    if (ext == ".jpg" || ext == ".jpeg")
                    {
                        objImage.Save(thumbPath, ImageFormat.Jpeg);
                    }
                    else if (ext == ".png")
                    {
                        objImage.Save(thumbPath, ImageFormat.Png);
                    }
                }

                return uniqueFilename + "-" + Path.GetFileNameWithoutExtension(FileData.FileName);
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
