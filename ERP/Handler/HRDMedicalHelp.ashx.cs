using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for MedicalHelp
    /// </summary>
    public class MedicalHelp : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var medival_list = (from medHelp in db.tblHRDMedicalHelps
                                select new { medHelp.ECode, medHelp.EmployeeName, medHelp.PatientName, medHelp.Relation, medHelp.HospitalName, medHelp.ChequeIssueDate, medHelp.ChequeNumber, medHelp.ReceiverName, medHelp.MobileNumber, medHelp.Amount }).ToList();



            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    medival_list = medival_list.Where(z => z.ChequeIssueDate.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    medival_list = medival_list.Where(z => z.ChequeIssueDate.Date >= fromDate.Date && z.ChequeIssueDate.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(medival_list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Medical Help List", "Medical Help List", "Medical Help List");
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