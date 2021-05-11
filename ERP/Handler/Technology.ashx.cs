using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for Technology
    /// </summary>
    public class Technology : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {

            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            int TechonologyGroupId = Convert.ToInt16(context.Request.QueryString["TGroupId"].ToString());

            ERPContext db = new ERPContext();

            var tec = (from t in db.tblTechnologies
                       join tg in db.tblTechnologiesGroups
                       on t.TechnologiesGroupId equals tg.Id
                       select new { tg.TechnologiesGroup, t.TechnologiesGroupId, t.Technologies, t.IsActive, ChangedOn = t.ChgDate });
            if (TechonologyGroupId != 0)
            {
                tec = tec.Where(p => p.TechnologiesGroupId == TechonologyGroupId);
            }

            var techList = (from t in tec
                            select new { t.TechnologiesGroup, t.Technologies, t.IsActive, t.ChangedOn }).ToList();
            DataTable dt = ERPUtilities.ConvertToDataTable(techList.ToList());
            ERPUtilities.ExportExcel(context, timezone, dt, "Technology List", "Technology List", "TechnologyList");
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