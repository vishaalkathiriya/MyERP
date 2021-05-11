using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace ERP.ActionFilter
{
    public class HasAccessRightFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Boolean flag = false;
            string AreaName = null;
            string ctrl = null ;
            string tempGetAreaname = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.Namespace;

            string[] temp = tempGetAreaname.Split('.');
            int i = temp.Count();

            if (i >= 3)
            {
                AreaName = temp[2];
            }


            if (! string.IsNullOrEmpty(AreaName))
            {
                ctrl = string.Concat(AreaName, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
            }
            else
            {
                ctrl = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            }


            foreach (int val in Enum.GetValues(typeof(ERPUtilities.AccessPermission)))
            {
                bool hasRights = ERPUtilities.HasAccessPermission(val, ctrl);
                if (hasRights)
                {
                    flag = true;
                    return;
                }
            }

            if (flag)
            {
                return;
            }
            else
            {
                // RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                //redirectTargetDictionary.Add("action", "Index");
                //redirectTargetDictionary.Add("controller", "Dashboard");
                //filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                string redirUrl = "/Dashboard/Index";
                filterContext.Result = new RedirectResult(redirUrl);
                return;
            }
            base.OnActionExecuting(filterContext);
        }



    }
}