using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.ActionFilter;
namespace ERP.Areas.Sarin.Controllers
{
    [HasAccessRightFilter]
    [HasLoginSessionFilter]
    [Audit]
    public class PurchaseController : Controller
    {
        //
        // GET: /Sarin/Purchase/

        // GET: /MedicalHelp/
        public ActionResult Index()
        {
            ViewBag.ctrlName = ControllerContext.RouteData.Values["Controller"].ToString();
            ViewBag.TempDocPdf = ConfigurationManager.AppSettings["TempUploadPath"].ToString();
            ViewBag.DocPdf = ConfigurationManager.AppSettings["UploadSarinPurchase"].ToString();
            return View();
        }

        [HttpPost]
        public string sarinPurchaseDocument(HttpPostedFileBase FileData)
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
