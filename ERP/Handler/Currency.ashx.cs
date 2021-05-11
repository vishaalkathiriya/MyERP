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
    /// Summary description for Currency
    /// </summary>
    public class Currency : IHttpHandler
    {
        int timezone;
        public void ProcessRequest(HttpContext context)
        {

            timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();


            //apply chnage

            var currencyList = db.tblCurrencies.Where(z=>z.IsDeleted ==  false).Select(
                z => new
                {
                   CurrencyName = z.CurrencyName,
                   CurrencyCode=z.CurrencyCode,
                   CountrtyName=db.tblCountries.Where(a=>a.CountryId == z.CountryId).Select(a=>a.CountryName).FirstOrDefault(),
                   IsActive=z.IsActive,
                   chgDate=z.ChgDate

                }).ToList().OrderBy(z => z.CurrencyName).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(currencyList);
            ERPUtilities.ExportExcel(context, timezone, dt, "Currency List", "Currency List", "CurrencyList");
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