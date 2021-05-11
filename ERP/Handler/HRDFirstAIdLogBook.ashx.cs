using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for FirstAidLogBook
    /// </summary>
    public class HRDFirstAIdLogBook : IHttpHandler
    {


        public void ProcessRequest(HttpContext context)
        {

            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var FirstAid = (from firstaid in db.tblHRDFirstAIdLogBooks
                            select new
                            {
                                firstaid.NameOfIssuer,
                                firstaid.NameOfReceiver,
                                firstaid.NameOfFirstAIdItems,
                                firstaid.DateOfIssue,
                                firstaid.Quanity,
                                Size = firstaid.Size == null ? 0 : firstaid.Size.Value,
                                firstaid.ManagerName,
                                firstaid.LocationOfFirstAIdBox,
                                Price = firstaid.Price == null ? 0 : firstaid.Price.Value,
                                firstaid.ExpiryDate,
                                Remarks = firstaid.Remarks == null ? " " : firstaid.Remarks,
                            }).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    FirstAid = FirstAid.Where(z => z.DateOfIssue.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    FirstAid = FirstAid.Where(z => z.DateOfIssue.Date >= fromDate.Date && z.DateOfIssue.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(FirstAid);
            ERPUtilities.ExportExcel(context, timezone, dt, "FirstAid Log Book List", "First Aid Log Book List", "First Aid Log Book List");
            context = null;
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