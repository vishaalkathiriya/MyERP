using ERP.Models;
using ERP.Utilities;
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
    public class ABTemplateController : ApiController
    {
         ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Template";

        public ABTemplateController()
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
        public ApiResponse CreateUpdateTemplate(tblABTemplate template)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (sessionUtils.HasUserLogin())
                {
                    try
                    {
                        NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                        // CREATE MODE
                        if (template.TemplateId == 0)
                        {
                            tblABTemplate tbl = new tblABTemplate
                            {
                                TemplateName = template.TemplateName,
                                TemplateFormate = template.TemplateFormate,
                                //IsActive = template.IsActive,
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblABTemplates.Add(tbl);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        }
                        else
                        {
                            // UPDATE MODE

                            tblABTemplate tbl = db.tblABTemplates.Where(z => z.TemplateId == template.TemplateId).FirstOrDefault();
                            tbl.TemplateName = template.TemplateName;
                            tbl.TemplateFormate = template.TemplateFormate;
                            //tbl.IsActive = template.IsActive;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

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

        /// <summary>
        /// Retrieive list template.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetriveTemplate()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {

                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblABTemplate> list = null;

                    try
                    {
                        list = db.tblABTemplates.ToList();

                        // FILTERING DATA
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TemplateName.ToLower().Contains(filter.ToLower())).ToList();
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
        public List<tblABTemplate> DoSorting(IEnumerable<tblABTemplate> list, string orderBy)
        {
            try
            {
                if (orderBy == "templateName")
                {
                    list = list.OrderBy(z => z.TemplateName).ToList();
                }
                else if (orderBy == "-templateName")
                {
                    list = list.OrderByDescending(z => z.TemplateName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblABTemplate>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Delete template by its primary key[templateId]
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse Deletetemplate([FromBody]int templateId)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblABTemplates.Where(z => z.TemplateId == templateId).SingleOrDefault();
                if (line != null)
                {
                    db.tblABTemplates.Remove(line);
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }

            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;

        }

      

        // Get ALL Template
        [HttpGet]
        public ApiResponse GetTemplateList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {

                    try
                    {
                        var templateList = db.tblABTemplates.ToList();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", templateList);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
    }
}
