using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    public class PressMediaExp : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var pressMediaExp_list = (from pressMediaExp in db.tblHRDPressMediaExpenses
                                     select new { pressMediaExp.NameOfPressMedia,pressMediaExp.RepresentativeName,pressMediaExp.Occasion,pressMediaExp.ApprovedBy,pressMediaExp.Amount,pressMediaExp.Date,pressMediaExp.MobileNumber  }).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    pressMediaExp_list = pressMediaExp_list.Where(z => z.Date.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    pressMediaExp_list = pressMediaExp_list.Where(z => z.Date.Date >= fromDate.Date && z.Date.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(pressMediaExp_list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Press Media Expense List", " Press Media Expense List", " Press Media Expense List");
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