using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP.Handler
{
    /// <summary>
    /// Export to excel handler for festival list 
    /// </summary>
    public class Festival : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //getting query string parameter
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            int FestivalTypeId = Convert.ToInt16(context.Request.QueryString["FestivalType"].ToString());
            ERPContext db = new ERPContext();

            List<FestivalViewModel> list = new List<FestivalViewModel>();

            //getting festival list
            var fGroupList = db.tblFestivals.GroupBy(p => p.FestivalGroupId, (key, g) => new { FestivalGroupId = key }).ToList();
            foreach (var grp in fGroupList)
            {
                var sLine = db.tblFestivals.Where(z => z.FestivalGroupId == grp.FestivalGroupId).OrderBy(z => z.FestivalId).Take(1).SingleOrDefault();
                var eLine = db.tblFestivals.Where(z => z.FestivalGroupId == grp.FestivalGroupId).OrderByDescending(z => z.FestivalId).Take(1).SingleOrDefault();
                int cnt = db.tblFestivals.Where(z => z.FestivalGroupId == grp.FestivalGroupId).Count();
                FestivalViewModel f = new FestivalViewModel
                {
                    FestivalName = sLine.FestivalName,
                    FestivalDate = cnt == 1 ? sLine.FestivalDate.ToShortDateString() : string.Format("{0} - {1}", sLine.FestivalDate.ToShortDateString(), eLine.FestivalDate.ToShortDateString()),
                    FestivalType = db.tblFestivalTypes.Where(z => z.FestivalTypeId == sLine.FestivalTypeId).Select(z => z.FestivalType).SingleOrDefault(),
                    FestivalTypeId = sLine.FestivalTypeId,
                    totalDays = cnt,
                    IsActive = sLine.IsActive,
                    ChgDate = sLine.ChgDate,
                };
                list.Add(f);
            }

            //apply filter on resulted data if festival type selection is exists
            if (FestivalTypeId != 0) {
                list = list.Where(z => z.FestivalTypeId == FestivalTypeId).ToList();
            }

            var selected = list.Select(z => new { z.FestivalName, z.FestivalDate, z.FestivalType, z.totalDays, z.IsActive, z.ChgDate }).OrderBy(z => z.FestivalName).ToList();

            //export list to excel
            DataTable dt = ERPUtilities.ConvertToDataTable(selected);
            ERPUtilities.ExportExcel(context, timezone, dt, "Festival List", "Festival List", "FestivalList");
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