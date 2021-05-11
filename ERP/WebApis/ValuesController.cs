using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;

namespace ERP.WebApis
{
    public class ValuesController : ApiController
    {
        private ERPContext db = new ERPContext();

        // GET api/<controller>
        public IEnumerable<tblDocument> Get()
        {
            return db.tblDocuments;
        }

        public IEnumerable<string> GetAll()
        {

            List<string> lstString = new List<string>();
            for (var i = 0; i < 50000; i++) {
                lstString.Add("value" + i.ToString() + "-GetAll");
            }

            return lstString;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post(DesignationData data)
        {

        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }


    public class DesignationData
    {
        public string name { get; set; }
        public int age { get; set; }
        public string tst { get; set; }
    }
}