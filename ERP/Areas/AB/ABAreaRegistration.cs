using System.Web.Mvc;

namespace ERP.Areas.AB
{
    public class ABAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AB";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AB_default",
                "AB/{controller}/{action}/{id}",
                new { controller = "Group", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
