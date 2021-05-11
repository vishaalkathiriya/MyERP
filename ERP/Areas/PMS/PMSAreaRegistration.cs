using System.Web.Mvc;

namespace ERP.Areas.PMS
{
    public class PMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "PMS_default",
            //    "PMS/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                name: "PMSModule",
                url: "PMS/{controller}/{id}",
                defaults: new { controller = "Project", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
