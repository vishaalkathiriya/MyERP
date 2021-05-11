using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ERP.Areas.Reception.Classes
{
    public class General
    {
        //Added by dipak(192.168.0.5)
        /// <summary>
        /// return datatable from query result.
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public static DataTable GetDatatableQuery(string Query, bool isreadonly = false)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReceptionContext_Tele"].ToString()))
            {
                Con.Open();
                using (SqlCommand command = new SqlCommand(Query, Con))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 0;

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        using (DataTable tb = new DataTable())
                        {
                            tb.Load(dr);
                            if (isreadonly)
                            {
                                foreach (DataColumn col in tb.Columns)
                                    col.ReadOnly = false;
                            }
                            return tb;
                        }
                    }
                }
            }
        }

        public static DataTable GetDatatableQuery_intContext(string Query, bool isreadonly = false)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["intContext"].ToString()))
            {
                Con.Open();
                using (SqlCommand command = new SqlCommand(Query, Con))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 0;

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        using (DataTable tb = new DataTable())
                        {
                            tb.Load(dr);
                            if (isreadonly)
                            {
                                foreach (DataColumn col in tb.Columns)
                                    col.ReadOnly = false;
                            }
                            return tb;
                        }
                    }
                }
            }
        }

        //Added by dipak(192.168.0.3)
        /// <summary>
        /// return datatable from query result.
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public static DataTable GetDatatableQuery_EmpDtl(string Query, bool isreadonly = false)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReceptionContext_Emp"].ToString()))
            //using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPContext"].ToString()))
            {
                Con.Open();
                using (SqlCommand command = new SqlCommand(Query, Con))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 0;

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        using (DataTable tb = new DataTable())
                        {
                            tb.Load(dr);
                            if (isreadonly)
                            {
                                foreach (DataColumn col in tb.Columns)
                                    col.ReadOnly = false;
                            }
                            return tb;
                        }
                    }
                }
            }
        }


        //Added by dipak(XE123)
        /// <summary>
        /// return datatable from query result.
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public static DataTable GetDatatableQuery_Teledata(string Query, bool isreadonly = false)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["ReceptionContext_Tele"].ToString()))
            {
                Con.Open();
                using (SqlCommand command = new SqlCommand(Query, Con))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 0;

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        using (DataTable tb = new DataTable())
                        {
                            tb.Load(dr);
                            if (isreadonly)
                            {
                                foreach (DataColumn col in tb.Columns)
                                    col.ReadOnly = false;
                            }
                            return tb;
                        }
                    }
                }
            }
        }
    }
}