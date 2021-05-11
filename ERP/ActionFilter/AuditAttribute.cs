using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Models;
using ERP.Utilities;

namespace ERP.ActionFilter
{
    public class AuditAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Store request
            var request = filterContext.HttpContext.Request;
            
            tblLOGPageActivity logActivity = new tblLOGPageActivity()
            {
                UserId = HttpContext.Current.Session["UserId"] != null ? Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()) : 0,
                IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                Url = request.RawUrl,
                VisitDate = DateTime.UtcNow
            };

            ERPUtils erpUtils = new ERPUtils();
            erpUtils.SaveLogActivity(logActivity);

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}