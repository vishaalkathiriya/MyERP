using ERP.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class GeneralController : ApiController
    {

        private ERPContext db = new ERPContext();

        [HttpGet]
        public IEnumerable<tblCountry> GetCountries()
        {
            try
            {
                return db.tblCountries.Where(z => z.IsActive == true).OrderBy(z => z.CountryName).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public IEnumerable<tblState> GetStatesByCountry(tblCountry country)
        {
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                //string countryName = nvc["countryName"];
                return db.tblStates.Where(z => z.tblCountry.CountryName == country.CountryName).OrderBy(z => z.StateName).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}