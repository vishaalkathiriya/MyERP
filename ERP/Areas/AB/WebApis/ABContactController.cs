using ERP.Models;
using ERP.Utilities;
using Excel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace ERP.WebApis
{
    public class ABContactController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Contact";

        public ABContactController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// Create / Update Contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateContact(tblABContact contact)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (sessionUtils.HasUserLogin())
                {
                    try
                    {
                        NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                        string[] selectedGroup = nvc["selectedGroup"].Split(',');
                        // CREATE MODE
                        if (contact.ContactId == 0)
                        {
                            tblABContact tbl = new tblABContact
                            {
                                Name = contact.Name,
                                PhoneNo = contact.PhoneNo,
                                LandlineNo = contact.LandlineNo,
                                Address1 = contact.Address1,
                                Address2 = contact.Address2,
                                Area = contact.Area,
                                City = contact.City,
                                Pincode = contact.Pincode,
                                CompanyName = contact.CompanyName,
                                Note = contact.Note,
                                LangId = contact.LangId,
                              
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblABContacts.Add(tbl);
                            db.SaveChanges();
                            int contactId = db.tblABContacts.Max(z => z.ContactId);
                            if (selectedGroup[0] != "")
                            {
                                for (int i = 0; i < selectedGroup.Length; i++)
                                {
                                    int groupId = Convert.ToInt32(selectedGroup[i]);
                                    tblABGrp_Contact tblGrp_Cnt = new tblABGrp_Contact
                                    {
                                        ContactId = contactId,
                                        GroupId = groupId
                                    };
                                    db.tblABGrp_Contact.Add(tblGrp_Cnt);
                                }
                            }
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                        else
                        {
                            // UPDATE MODE

                            tblABContact tbl = db.tblABContacts.Where(z => z.ContactId == contact.ContactId).FirstOrDefault();
                            tbl.Name = contact.Name;
                            tbl.CompanyName = contact.CompanyName;
                            tbl.PhoneNo = contact.PhoneNo;
                            tbl.LandlineNo = contact.LandlineNo;
                            tbl.Address1 = contact.Address1;
                            tbl.Address2 = contact.Address2;
                            tbl.Area = contact.Area;
                            tbl.City = contact.City;
                            tbl.Pincode = contact.Pincode;
                            tbl.Note = contact.Note;
                            tbl.LangId = contact.LangId;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                            var line = db.tblABGrp_Contact.Where(z => z.ContactId == contact.ContactId);
                            if (line != null)
                            {
                                foreach (var detail in line)
                                {
                                    db.tblABGrp_Contact.Remove(detail);
                                }
                            }
                            if (selectedGroup[0] != "")
                            {
                                for (int i = 0; i < selectedGroup.Length; i++)
                                {
                                    int groupId = Convert.ToInt32(selectedGroup[i]);
                                    tblABGrp_Contact tblGrp_Cnt = new tblABGrp_Contact
                                    {
                                        ContactId = contact.ContactId,
                                        GroupId = groupId
                                    };
                                    db.tblABGrp_Contact.Add(tblGrp_Cnt);
                                }
                            }
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                        }
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                }
                else
                {
                    ERPUtilities.UnAuthorizedAccess(apiResponse);
                }

            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        }

        [HttpPost]
        public ApiResponse DeleteContact([FromBody]int ContactId)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblABGrp_Contact.Where(z => z.ContactId == ContactId);
                if (line != null)
                {
                    foreach (var detail in line)
                    {
                        db.tblABGrp_Contact.Remove(detail);
                    }
                }
                var linecnt = db.tblABContacts.Where(z => z.ContactId == ContactId).SingleOrDefault();
                if (linecnt != null)
                {
                    db.tblABContacts.Remove(linecnt);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        }

       
        /// <summary>
        /// Apply Group To Contact
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse ApplyGroupcontact()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (sessionUtils.HasUserLogin())
                {
                    using (db)
                    {
                        NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                        string[] selectedGroup = nvc["selectedGroup"].Split(',');
                        string[] selectedContact = nvc["selectedContact"].Split(',');

                        for (int i = 0; i < selectedGroup.Length; i++)
                        {
                            int groupId = Convert.ToInt32(selectedGroup[i]);
                            for (int sc = 0; sc < selectedContact.Length; sc++)
                            {
                                int contactId = Convert.ToInt32(selectedContact[sc]);

                                var line = db.tblABGrp_Contact.Where(z => z.ContactId == contactId && z.GroupId == groupId).SingleOrDefault();
                                if (line == null)
                                {
                                    tblABGrp_Contact tblGrp_Cnt = new tblABGrp_Contact
                                    {
                                        ContactId = contactId,
                                        GroupId = groupId
                                    };
                                    db.tblABGrp_Contact.Add(tblGrp_Cnt);
                                }
                            }
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                        db.SaveChanges();
                    }

                }
                else
                {
                    ERPUtilities.UnAuthorizedAccess(apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        }

        /// <summary>
        /// Retrieive list Contact.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveContact()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {

                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"],
                    name = nvc["name"],
                    address = nvc["address"],
                    phoneNo = nvc["phoneNo"],
                    prmgroupName = nvc["groupName"].Remove(nvc["groupName"].Trim().Length - 1);
                    int[] groupNameId={};
                    string[] groupName = (prmgroupName).Split(',');
                   
                    if (!string.IsNullOrEmpty(prmgroupName) && prmgroupName != "undefined")
                    {

                        groupNameId = Array.ConvertAll(groupName, int.Parse);
                    }
                    int langId = Convert.ToInt32(nvc["langId"]);
                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;
                    int group = 0;
                    List<ABContactViewModel> list = null;

                    try
                    {
                        var groupId = new SqlParameter
                        {
                            ParameterName = "groupId",
                            Value = 0
                        };
                        list = db.Database.SqlQuery<ABContactViewModel>("tblABContactSelectAll @groupId", groupId).ToList();

                        //list = db.tblABContacts.ToList();

                        // FILTERING DATA BASIS ON CONTACT NAME
                        if (!string.IsNullOrEmpty(name) && name != "undefined")
                        {
                           
                            list = list.Where(z => z.Name.ToLower().Contains(name.ToLower())).ToList();
                        }
                        // FILTERING DATA BASIS ON CONTACT COMPANY NAME
                        if (!string.IsNullOrEmpty(address) && address != "undefined")
                        {
                          
                            list = list.Where(z => z.AreaCity.ToLower().Contains(address.ToLower())).ToList();
                        }
                        // FILTERING DATA BASIS ON CONTACT PHONENO.
                        if (!string.IsNullOrEmpty(phoneNo) && phoneNo != "undefined")
                        {
                          
                            list = list.Where(z => z.PhoneNo.ToLower().Contains(phoneNo.ToLower())).ToList();
                        }
                        // FILTERING DATA BASIS ON CONTACT GroupName
                        if (groupNameId.Length > 0 )
                        {

                            list = (from l in list
                                   join cg in db.tblABGrp_Contact on l.ContactId equals cg.ContactId
                                   where groupNameId.Contains(cg.GroupId)
                                   select new ABContactViewModel
                                   {
                                       ContactId = l.ContactId,
                                       Name= l.Name,
                                       PhoneNo=l.PhoneNo,
                                       LandlineNo=l.LandlineNo,
                                       Address1=l.Address1,
                                       Address2=l.Address2,
                                       Area=l.Area,
                                       City=l.City,
                                       Pincode=l.Pincode,
                                       CompanyName=l.CompanyName,
                                       Note=l.Note,
                                       LangId=l.LangId,
                                       CreBy=l.CreBy,
                                       ChgBy=l.ChgBy,
                                       CreDate=l.CreDate,
                                       ChgDate=l.ChgDate,
                                       GroupName=l.GroupName,
                                       AreaCity=l.AreaCity
                                   }).ToList();
                            list = list.GroupBy(id => id.ContactId).Select(con => con.First()).ToList();
                        }
                        // FILTERING DATA BASIS ON LANGUAGE
                        if (langId != 0)
                        {
                           
                            list = list.Where(z => z.LangId==langId).ToList();
                        }

                        // SORTING DATA
                        list = DoSorting(list, orderBy.Trim());

                        // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                        var Count = list.Count();

                        // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = list.ToList()
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public List<ABContactViewModel> DoSorting(IEnumerable<ABContactViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "Name")
                {
                    list = list.OrderBy(z => z.Name).ToList();
                }
                else if (orderBy == "-Name")
                {
                    list = list.OrderByDescending(z => z.Name).ToList();
                }
                else if (orderBy == "AreaCity")
                {
                    list = list.OrderBy(z => z.AreaCity).ToList();
                }
                else if (orderBy == "-AreaCity")
                {
                    list = list.OrderByDescending(z => z.AreaCity).ToList();
                }
                else if (orderBy == "PhoneNo")
                {
                    list = list.OrderBy(z => z.PhoneNo).ToList();
                }
                else if (orderBy == "-PhoneNo")
                {
                    list = list.OrderByDescending(z => z.PhoneNo).ToList();
                }
                else if (orderBy == "GroupName")
                {
                    list = list.OrderBy(z => z.GroupName).ToList();
                }
                else if (orderBy == "-GroupName")
                {
                    list = list.OrderByDescending(z => z.GroupName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<ABContactViewModel>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieive list Contact.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse GetPrintAddress([FromBody]int group)
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {

                using (db)
                {
                    List<ABContactViewModel> list = null;

                    try
                    {
                        var groupId = new SqlParameter
                        {
                            ParameterName = "groupId",
                            Value = group
                        };
                        list = db.Database.SqlQuery<ABContactViewModel>("tblABContactSelectAll @groupId", groupId).ToList();

                        var resultData = new
                        {
                            result = list.ToList()
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        
        /// <summary>
        /// Import Contact From Excelsheet 
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public ApiResponse ImportContact()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (sessionUtils.HasUserLogin())
                {

                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    OleDbConnection oledbConn = new OleDbConnection();
                    string fileName = nvc["fileName"];
                    string fileExtension = Path.GetExtension(fileName);
                    string path = ConfigurationManager.AppSettings["TempABContactExcelPath"].ToString();
                    string fullpath = Path.GetFullPath(System.Web.HttpContext.Current.Server.MapPath("~/" + path + "/" + fileName));

                    // Set Connection String 

                    if (Path.GetExtension(fullpath) == ".xls")
                    {
                        oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"");
                    }
                    else if (Path.GetExtension(fullpath) == ".xlsx")
                    {
                        oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
                    }

                    oledbConn.Open();
                    OleDbCommand cmd = new OleDbCommand(); ;
                    OleDbDataAdapter da = new OleDbDataAdapter();
                    DataTable dt = new DataTable();

                    // Fetch Data From Excel Sheet

                    cmd.CommandText = "Select * from [Contact$]";

                    cmd.Connection = oledbConn;
                    da.SelectCommand = cmd;
                    da.Fill(dt);

                    // set Data into tblABContact

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        tblABContact tblAbContact = new tblABContact();
                        tblAbContact.Name = dt.Rows[i]["Name"].ToString();
                        tblAbContact.PhoneNo = dt.Rows[i]["Phone No"].ToString();
                        tblAbContact.LandlineNo = dt.Rows[i]["Landline No"].ToString();
                        tblAbContact.Address1 = dt.Rows[i]["Line 1"].ToString();
                        tblAbContact.Address2 = dt.Rows[i]["Line 2"].ToString();
                        tblAbContact.City = dt.Rows[i]["City"].ToString();
                        tblAbContact.Area = dt.Rows[i]["Area"].ToString();
                        tblAbContact.Pincode = dt.Rows[i]["Pincode"].ToString();
                        tblAbContact.Note = dt.Rows[i]["Note"].ToString();
                        tblAbContact.LangId = Convert.ToInt32(dt.Rows[i]["Language Id"]);
                        tblAbContact.CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        tblAbContact.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        tblAbContact.CreDate = DateTime.Now.ToUniversalTime();
                        tblAbContact.ChgDate = DateTime.Now.ToUniversalTime();
                        db.tblABContacts.Add(tblAbContact);
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                }
                else
                {
                    ERPUtilities.UnAuthorizedAccess(apiResponse);
                }

            }
            catch (DbEntityValidationException e)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(e, _pageName, true);
            }
            return apiResponse;
        }

    }
}