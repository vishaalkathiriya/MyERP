using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Web;
using ERP.Models;
using ERP.Utilities;
using System.Configuration;

namespace ERP.WebApis
{
    public class LoginController : ApiController
    {
        ERPContext db = new ERPContext();

        [HttpPost]
        public ApiResponse IsAuthenticatedUser(UserLogin userLogin)
        {
            ApiResponse apiResponse = new ApiResponse();
            string baseUrl = string.Empty;
            bool IsAuth = false;
            try
			{
                baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                var list = (from login in db.tblEmpLoginInformations
                            join p in db.tblEmpPersonalInformations on login.EmployeeId equals p.EmployeeId
                            where login.UserName == userLogin.Username && login.Password == userLogin.Password
                            select new { login.EmployeeId, login.UserName, login.IsRemoteLogin, p.CandidateFirstName, p.CandidateMiddleName, p.CandidateLastName, p.ProfilePhoto }).FirstOrDefault();
                if (list != null)
                {
                    string[] parts = baseUrl.Replace("http://", "").Split(':');
                    if (parts[0].ToLower() == ConfigurationManager.AppSettings["HostName"].ToString().ToLower()) {//Same Host: Don't check for remote login 
                        IsAuth = true;
                        userLogin.UserId = list.EmployeeId;
                        ERPUtilities.ToggleUserLoginStatus(userLogin.UserId, true);
                        HttpContext.Current.Session["UserId"] = userLogin.UserId;
                        HttpContext.Current.Session["UserName"] = string.Format("{0}{1}{2}", list.CandidateFirstName, list.CandidateMiddleName.Substring(0,1).ToUpper(), list.CandidateLastName.Substring(0,1).ToUpper());
                        HttpContext.Current.Session["ProfilePhoto"] = list.ProfilePhoto;
                        HttpContext.Current.Session["MenuView"] = "S"; //Side Bar - left side
                    }
                    else { //Different Host: Check for remote login permission
                        if (list.IsRemoteLogin) {
                            IsAuth = true;
                            userLogin.UserId = list.EmployeeId;
                            ERPUtilities.ToggleUserLoginStatus(userLogin.UserId, true);
                            HttpContext.Current.Session["UserId"] = userLogin.UserId;
                            HttpContext.Current.Session["UserName"] = list.CandidateFirstName + " " + list.CandidateLastName;
                            HttpContext.Current.Session["ProfilePhoto"] = list.ProfilePhoto;
                            HttpContext.Current.Session["MenuView"] = "S"; //Side Bar - left side
                        }
                    }


                    //Save LogActivity
                    var request = new HttpRequestWrapper(HttpContext.Current.Request);
                    tblLOGPageActivity logActivity = new tblLOGPageActivity()
                    {
                        UserId = HttpContext.Current.Session["UserId"] != null ? Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()) : 0,
                        IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                        Url = request.RawUrl,
                        VisitDate = DateTime.UtcNow
                    };
                    ERPUtils erpUtils = new ERPUtils();
                    erpUtils.SaveLogActivity(logActivity);


                    apiResponse = IsAuth ? ERPUtilities.GenerateApiResponse(true, 1, "", "") : ERPUtilities.GenerateApiResponse(true, 2, "", "");
                }
                else
                {
                    ERPUtilities.GenerateApiResponse(true, 0, "", "");
                }
            }
            catch (Exception ex) {
                //apiResponse = ERPUtilities.GenerateApiResponse();
                apiResponse = ERPUtilities.GenerateApiResponse(false, -1, "", "");
            }
            
            return apiResponse;
        }

        protected void InitializeSession(object list, UserLogin userLogin) {
            
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
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
}