using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ERP.ActionFilter
{
    public class HasLoginSessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserId"] == null)
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("area", "");
                redirectTargetDictionary.Add("action", "Index");
                redirectTargetDictionary.Add("controller", "Login");
                filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                return;
            }
        }
    }
}