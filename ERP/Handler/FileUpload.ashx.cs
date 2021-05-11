using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for FileUpload
    /// </summary>
    public class FileUpload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<string> uniqueFilenames = new List<string>();
            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 1)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];
                        string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(file.FileName));
                        string tempMainPath = Path.Combine(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);
                        //string tempThumbPath = Path.Combine(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);
                        file.SaveAs(tempMainPath);
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    HttpPostedFile file = files[0];
                    string uniqueFilename = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), Path.GetExtension(file.FileName));
                    string mainPath = Path.Combine(HttpContext.Current.Server.MapPath("../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), uniqueFilename);
                    file.SaveAs(mainPath);

                    uniqueFilenames.Add(uniqueFilename);
                }
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(string.Join(",", uniqueFilenames.ToArray()));
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}