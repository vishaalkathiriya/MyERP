using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Controllers
{
    public class WebCamController : Controller
    {
        //
        // GET: /WebCam/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult Capture(string fileData)
        {
            var base64ImageString = fileData.Split(';')[1].Split(',')[1];

            Image img = Base64ToImage(base64ImageString);
            string imageName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
            string imagePath = string.Format("~/webcamimages/{0}.png", imageName);
            var path = Server.MapPath(imagePath);
            img.Save(path);

            return Content("success");
        }

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

    }
}
