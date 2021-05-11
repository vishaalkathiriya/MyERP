using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.ActionFilter;


namespace ERP.Areas.HR.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/
        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            return View();
        }

        //
        // GET: /Employee/PersonalInformation/id

        public ActionResult Create(int? id)
        {

            if (Request.UrlReferrer == null || id == 1)
            {
                Response.Redirect("/HR/Employee");
            }

            ViewBag.ID = id;
            ViewBag.TempImageThumbPath = "../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString();
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();

            ViewBag.mainDocPath = "../../" + ConfigurationManager.AppSettings["empDocUploads"].ToString();
            ViewBag.thumbDocPath = "../../" + ConfigurationManager.AppSettings["empDocThumbnails"].ToString();
            ViewBag.empPDFDoc = "../../" + ConfigurationManager.AppSettings["empPDFUploads"].ToString();
            
            return View();

        }


        [HttpPost]
        public string ProPic(HttpPostedFileBase FileData)
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
                //Image objImage = ScaleImage(bmpPostedImage, 100, 100);
                Image objImage = getDrcAutoConvertImage(bmpPostedImage, 100);
                objImage.Save(thumbPath, ImageFormat.Jpeg);

                return uniqueFilename;
            }
            catch
            {
                return string.Empty;
            }
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

                if(Path.GetExtension(FileData.FileName) != ".pdf")
                {

                //create thumbnail and save
                Bitmap bmpPostedImage = new System.Drawing.Bitmap(FileData.InputStream);
                Image objImage = ScaleImage(bmpPostedImage, 321, 200);
                //Image objImage = getDrcAutoConvertImage(bmpPostedImage, 100);
                objImage.Save(thumbPath, ImageFormat.Jpeg);
                }
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
            //Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static Image getDrcAutoConvertImage(Image img, Int32 iSize)
        {
            Int32 tHeight = 0, tWidth = 0;
            Int32 posX = 0, posY = 0;

            Bitmap normalLarge = new Bitmap(iSize, iSize);
            Graphics grThumbnail = Graphics.FromImage(normalLarge);
            grThumbnail.Clear(Color.White);
            if (img.Height > img.Width)
            {
                tHeight = iSize;
                tWidth = iSize * img.Width / img.Height;
                posX = (iSize - tWidth) / 2;
            }
            else
            {
                tHeight = iSize * img.Height / img.Width;
                tWidth = iSize;
                posY = (iSize - tHeight) / 2;
            }


            grThumbnail.DrawImage(img, new Rectangle(posX, posY, tWidth, tHeight), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            return (Image)normalLarge;

        }

    }
}
