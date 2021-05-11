

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data;
using System.Data.SqlClient;
using ERP.Areas.Reception.Classes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.SqlTypes;

namespace ERP.Areas.Reception.WebApis
{
    public class VisitorMasterController : ApiController
    {
        ERPContext db = null;
        intContext intDb = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Visitor Record";

        public VisitorMasterController()
        {
            intDb = new intContext();
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/EmployeeDetailReport
        /// retrieve Dept list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveCompany()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();
                    //For ReceptionContext_Emp
                    //dt = General.GetDatatableQuery_intContext("select dept from Employee where gname in ('dd','mg','xi') group by Dept order by Dept");

                    //ERPContext
                    dt = General.GetDatatableQuery_intContext("Select Company From Visitormaster Group By Company Order By Company");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveDesignation()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();
                    //For ReceptionContext_Emp
                    //dt = General.GetDatatableQuery_intContext("select dept from Employee where gname in ('dd','mg','xi') group by Dept order by Dept");

                    //ERPContext
                    dt = General.GetDatatableQuery_intContext("Select Designation From Visitormaster Group By Designation Order By Designation");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveEcode()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();

                    dt = General.GetDatatableQuery_intContext("Select Ecode from Employeemaster where AMCode = 'VST' and Designation = 'RECEPTION' and ecode not in (Select Ecode from Visitorinout where outtime is null) order by Ecode");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveRefName()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();

                    dt = General.GetDatatableQuery_intContext("Select RefName From VisitorInOut Group By Refname Order By refName");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveManager()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();

                    dt = General.GetDatatableQuery_intContext("Select manager from VisitorInOut Group by Manager order by Manager");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveDept()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();

                    dt = General.GetDatatableQuery_intContext("Select Param1 from Parameter Where Type='DEPARTMENT' Group by Param1 order by Param1");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

       
        public string GetString(byte[] bytes, DateTime pdate)
        {
            string returnString = string.Empty;
            
            try
            {
                if (pdate.Year == 2014 && pdate.Month == 12 && pdate.Day >= 22 && pdate.Day <= 26)
                {
                    char[] chars = new char[bytes.Length / sizeof(char)];
                    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
                    returnString = new string(chars);
                }
                if (pdate.Year > 2014)
                {
                    //if (pdate.Day >= 3 && pdate.Month >= 4)
                    //{
                    //    returnString = Convert.ToBase64String(bytes, 0, bytes.Length);
                    //}
                    //else
                    //{
                    char[] chars = null;
                    chars = new char[bytes.Length / sizeof(char)];
                    System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
                    returnString = new string(chars);
                    // }
                }
                else
                {
                    string JpgPath = HttpContext.Current.Server.MapPath(@"\VisitorImages\") + DateTime.Now.Ticks + "_save.jpg";
                    System.IO.File.WriteAllBytes(JpgPath, bytes);

                    byte[] targetto = System.IO.File.ReadAllBytes(JpgPath);
                    string pngPath = HttpContext.Current.Server.MapPath(@"\VisitorImages\") + DateTime.Now.Ticks + "_save.png";
                    System.IO.File.WriteAllBytes(pngPath, targetto);

                    byte[] taget = System.IO.File.ReadAllBytes(pngPath);
                    System.IO.File.Delete(pngPath);
                    returnString = Convert.ToBase64String(taget);
                }
            }
            catch (Exception ex)
            {
                returnString = string.Empty;
            }
            return returnString;

            //string pngPath = HttpContext.Current.Server.MapPath(@"\VisitorImages\") + DateTime.Now.Ticks + "_save.png";
            //System.IO.File.WriteAllBytes(pngPath, bytes);
            //byte[] toBytesIamge = System.IO.File.ReadAllBytes(pngPath);
            //System.IO.File.Delete(pngPath);
            //return Convert.ToBase64String(toBytesIamge);
            //char[] chars = new char[bytes.Length / sizeof(char)];
            //System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            //return new string(chars);

            //char[] chars = new char[bytes.Length / sizeof(char)];
            //System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            //return new string(chars);
        }



        [HttpGet]
        public ApiResponse GetVisitorData()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                    string MobileNo = nvc["MobileNo"];
                    DataTable dt = new DataTable();

                    //ERPContext
                    dt = General.GetDatatableQuery_intContext("Select Top 1 * from VisitorMaster where MobileNo='" + MobileNo + "' and IsActive='True'  Order by VisitorId Desc");
                    removeSeprator(dt);
                    decimal m = Convert.ToDecimal(MobileNo);
                    //dt.Rows[0]["Photo"] = Convert.ToBase64String(ObjectToByteArray(dt.Rows[0]["Photo"]));
                    //string base64 = Convert.ToBase64String(img);
                    var data = intDb.VisitorMasters.Where(z => z.MobileNo == m && z.IsActive == true).FirstOrDefault();
                    byte[] arr = data != null ? data.Photo : null;
                    DateTime Pdate = Convert.ToDateTime(intDb.VisitorMasters.Where(z => z.MobileNo == m).FirstOrDefault().PDate);
                    string imageBase64 = "";
                    dt.Columns.Add("Photo1", typeof(string));
                    if (arr != null)
                    {
                        //if (Pdate.Year == 2014 && Pdate.Month == 12 && Pdate.Day == 1)
                        //{
                        //    imageBase64 = Convert.ToBase64String(arr);
                        //}
                        //else
                        //{
                        imageBase64 = GetString(arr, Pdate);
                        //}
                        string imageSrc = string.Format("data:image/png;base64,{0}", imageBase64);

                        dt.Rows[0]["Photo1"] = imageBase64;
                    }
                    else
                    {
                        dt.Rows[0]["Photo1"] = null;
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        [HttpGet]
        public ApiResponse GetVisitorDataByVisitorID()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                    string VisitorID = nvc["VisitorID"];
                    DataTable dt = new DataTable();
                    //For ReceptionContext_Emp
                    //dt = General.GetDatatableQuery_intContext("select dept from Employee where gname in ('dd','mg','xi') group by Dept order by Dept");

                    //ERPContext
                    dt = General.GetDatatableQuery_intContext("select * from VisitorMaster where VisitorId='" + VisitorID + "' and IsActive='True'");

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        public class VisitorMasterVieModel
        {
            public int VisitorId { get; set; }
            public decimal MobileNo { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Company { get; set; }
            public string Designation { get; set; }
            public Nullable<System.DateTime> PDate { get; set; }
            public string Remark { get; set; }
            public string Photo { get; set; }
        }


        public byte[] GetBytes(string str)
        {
            //string pngPath = HttpContext.Current.Server.MapPath(@"\VisitorImages\") + DateTime.Now.Ticks + "_save.jpg";
            //byte[] Source = Convert.FromBase64String(str);
            //System.IO.File.WriteAllBytes(pngPath, Source);
            //byte[] target = System.IO.File.ReadAllBytes(pngPath);
            //System.IO.File.Delete(pngPath);
            //return target;

            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }




        [HttpPost]
        public ApiResponse CreateUpdateVisitor(VisitorMasterVieModel obj)
        {
            ApiResponse apiResponse = new ApiResponse();
            DataTable dt;
            int UserId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

            if (sessionUtils.HasUserLogin())
            {
                try
                {

                    if (obj.VisitorId == 0)
                    {
                        var mobileNo = intDb.VisitorMasters.Where(z => z.MobileNo == obj.MobileNo && z.IsActive == true).FirstOrDefault();
                        if (mobileNo != null)
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "Mobile Number Exist", null);

                        }
                        else
                        {
                            //dt = General.GetDatatableQuery_intContext("Select top 1 * from visitormaster where left(visitorid,4) = Year(getdate()) order by VisitorId Desc");
                            dt = General.GetDatatableQuery_intContext("Select top 1 * from VisitorMaster where left(visitorid,4) = Year(getdate()) order by VisitorId Desc");
                            if (dt.Rows.Count > 0)
                            {
                                obj.VisitorId = Convert.ToInt32(dt.Rows[0]["VisitorId"]) + 1;
                            }
                            else
                            {
                                obj.VisitorId = Convert.ToInt32(DateTime.Now.Year + "0001");
                            }
                            byte[] toBytesIamge = null;
                            string photo = obj.Photo == null ? null : obj.Photo.Split(',')[1];


                            if (photo != null)
                            {
                                toBytesIamge = GetBytes(photo);
                                //string pngPath = HttpContext.Current.Server.MapPath(@"\VisitorImages\") + DateTime.Now.Ticks + "_save.jpg";
                                //System.IO.File.WriteAllBytes(pngPath, toBytesIamge);
                                //toBytesIamge = System.IO.File.ReadAllBytes(pngPath);
                                //System.IO.File.Delete(pngPath);
                            }
                            else
                            {
                                toBytesIamge = null;
                            }
                            VisitorMaster objMaster = new VisitorMaster()
                            {
                                VisitorId = obj.VisitorId,
                                MobileNo = obj.MobileNo,
                                Name = obj.Name,
                                Address = obj.Address,
                                Company = obj.Company,
                                Designation = obj.Designation,
                                PDate = DateTime.Now,
                                Remark = obj.Remark != null ? obj.Remark : "",
                                Photo = toBytesIamge,
                                IsActive = true
                            };
                            intDb.VisitorMasters.Add(objMaster);
                            intDb.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "Visitor Registered", dt);
                        }
                    }
                    else
                    {
                        byte[] toBytesIamge = null;
                        var data = intDb.VisitorMasters.Where(z => z.MobileNo == obj.MobileNo && z.VisitorId == obj.VisitorId).FirstOrDefault();
                        if (data != null)
                        {
                            data.Name = obj.Name;
                            data.Address = obj.Address;
                            data.Company = obj.Company;
                            data.Remark = obj.Remark;
                            data.Designation = obj.Designation;
                            data.PDate = DateTime.Now;
                            if (obj.Photo == null)
                            {
                                data.Photo = data.Photo;
                            }
                            else
                            {
                                string photo = string.Empty;
                                if (obj.Photo.Contains("data:image/png;base64,"))
                                {
                                    photo = obj.Photo.Split(',')[1];
                                }
                                else
                                {
                                    photo = obj.Photo;
                                }
                                toBytesIamge = GetBytes(photo);
                                data.Photo = toBytesIamge;

                            }

                        }
                        intDb.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "Visitor Updated", null);
                    }
                }


                catch (Exception ex)
                {
                }
            }
            else
            {

            }

            return apiResponse;
        }

        [HttpPost]
        public ApiResponse VisitorIn(VisitorInOut obj, string mobileNo)
        {
            ApiResponse apiResponse = new ApiResponse();
            DataTable dt;
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    dt = General.GetDatatableQuery_intContext("Select * from VisitorInOut Where (Ecode = " + obj.ECode + " or VisitorId = " + obj.VisitorId + ") and Outtime is null");
                    if (dt.Rows.Count > 0)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "This user Already Exist", null);
                    }
                    else
                    {
                        if (mobileNo == null)
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 3, "Please Select Mobile No.", null);
                        }
                        else
                        {
                            dt = General.GetDatatableQuery_intContext("Select top 1 srno from VisitorInout order by srno desc");
                            if (dt.Rows.Count > 0)
                            {
                                obj.SrNo = Convert.ToInt32(dt.Rows[0]["srno"]) + 1;
                            }
                            else
                            {
                                obj.SrNo = 1;
                            }


                            intDb.Database.ExecuteSqlCommand("insert into VisitorInOut(SrNo, VisitorId,ECode, RefName, Department,Manager, ExtNo, InTime, OutTime,Reason, Person, Remark) values(" + obj.SrNo + "," + obj.VisitorId + "," + obj.ECode + ",'" + obj.RefName + "','" + obj.Department + "','" + obj.Manager + "'," + obj.ExtNo + ",getdate(),null," + "'" + obj.Reason + "'," + obj.Person + ",null)");

                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "Visitor In SucessFully", null);
                        }
                    }

                }
                catch (Exception ex)
                {
                }
            }
            else
            {

            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse GetInpersonList()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DataTable dt = new DataTable();

                    dt = General.GetDatatableQuery_intContext("Select SrNo,VisitorId,(Select top 1 Name from Visitormaster i where i.visitorid = visitorinout.visitorid) as VisitorName,InTime,ECode,RefName,Person,Department,Manager,ExtNo,Reason,CAST(ROW_NUMBER() over (Order by srno) AS int) HistID from Visitorinout where outtime is null");
                    dt.Columns.Add("MobileNo", typeof(decimal));
                    for (int index = 0; index < dt.Rows.Count; index++)
                    {
                        decimal VisitorID = Convert.ToInt32(dt.Rows[index]["VisitorId"]);
                        decimal mobileNo = intDb.VisitorMasters.Where(z => z.VisitorId == VisitorID).FirstOrDefault().MobileNo;
                        dt.Rows[index]["MobileNo"] = mobileNo;

                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", dt);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }
        [HttpPost]
        public ApiResponse VisitorOut()
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            DataTable dt;
            int SrNo = Convert.ToInt32(nvc["SrNo"]);
            string RefName = nvc["RefName"];
            int Ecode = Convert.ToInt32(nvc["Ecode"]);
            int VisitorId = Convert.ToInt32(nvc["VisitorId"]);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var data = intDb.VisitorInOuts.Where(z => z.VisitorId == VisitorId && z.RefName.Equals(RefName) && z.ECode == Ecode && z.SrNo == SrNo).FirstOrDefault();

                    if (data != null)
                    {
                        intDb.Database.ExecuteSqlCommand("update VisitorInout set outtime = getdate() where visitorid = " + data.VisitorId + " and SrNo = " + data.SrNo + " And Outtime Is Null");
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "Visitor Out SucessFully.", null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "This Person VisitorId, RefName And ECode Aren't Match With InEntry.", null);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        public void removeSeprator(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string address = dt.Rows[i]["Address"].ToString();
                    dt.Rows[i]["Address"] = address.Replace("{}", ",");
                }
            }
        }

        [HttpPost]
        public ApiResponse DeleteVisitorData(VisitorMaster obj)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = intDb.VisitorMasters.Where(z => z.VisitorId == obj.VisitorId).SingleOrDefault();
                    if (line != null)
                    {
                        line.IsActive = false;
                        // intDb.VisitorMasters.Remove(line);
                    }
                    intDb.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);

                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        [AcceptVerbs("GET", "POST")]
        public ApiResponse GetVisitorDetail()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (intDb)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string VisitorId = nvc["VisitorId"];
                    string Mobile = nvc["Mobile"];
                    string Name = nvc["Name"];
                    string Address = nvc["Address"];
                    string Company = nvc["Company"];
                    string Designation = nvc["Designation"];


                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    DataTable dt = new DataTable();
                    try
                    {
                        string Qry = "";


                        StringBuilder Query = new StringBuilder();
                        Query.Clear();
                        Query.Append(" 1=1 ");

                        //filter Visitor
                        if (!string.IsNullOrEmpty(VisitorId) && VisitorId != "undefined" && VisitorId != "") Query.Append(" And VisitorId like '%" + VisitorId + "%' ");

                        //filter Mobile
                        if (!string.IsNullOrEmpty(Mobile) && Mobile != "undefined" && Mobile != "") Query.Append(" And MobileNo like '%" + Mobile + "%' ");

                        //Filter name
                        if (!string.IsNullOrEmpty(Name) && Name != "undefined" && Name != "") Query.Append(" And Name like '%" + Name + "%' ");

                        //Filter Department
                        if (!string.IsNullOrEmpty(Address) && Address != "undefined" && Address != "") Query.Append(" And Address like '%" + Address + "%' ");

                        //Filter comapny
                        if (!string.IsNullOrEmpty(Company) && Company != "undefined" && Company != "") Query.Append(" And Company like '%" + Company + "%' ");

                        //Filter designation
                        if (!string.IsNullOrEmpty(Designation) && Designation != "undefined" && Designation != "") Query.Append(" And Designation like '%" + Designation + "%' ");

                        if (Query.ToString().Equals(" 1=1 ") == false)
                            dt = General.GetDatatableQuery_intContext("select * from VisitorMaster where " + Query.ToString() + " and  IsActive='True' order by Name");
                        else
                            dt = General.GetDatatableQuery_intContext("Select Top 50 * from VisitorMaster i Inner Join (Select VisitorId, Count(VisitorId) as Cnt from VisitorInOut Where inTime >= dateadd(d,-7,getdate()) Group by VisitorId ) as k on i.VisitorId = k.VisitorId where i.IsActive='True' Order by k.Cnt desc");
                        removeSeprator(dt);


                        //Sorting Datatable
                        if (orderBy.Trim().Contains("-"))
                        {//descending
                            orderBy = orderBy.Trim().Replace("-", "");
                            var Rows = (from row in dt.AsEnumerable()
                                        orderby row[orderBy] descending
                                        select row);
                            dt = Rows.AsDataView().ToTable();
                        }
                        else
                        {//ascending
                            orderBy = orderBy.Trim().Replace("-", "");
                            var Rows = (from row in dt.AsEnumerable()
                                        orderby row[orderBy] ascending
                                        select row);
                            dt = Rows.AsDataView().ToTable();
                        }



                        var count = dt.Rows.Count;

                        var resultData = new
                        {
                            total = count,
                            result = count > 0 ? dt.AsEnumerable().Skip(iDisplayStart).Take(iDisplayLength).CopyToDataTable() : dt
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);

                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {

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
