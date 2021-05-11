using System.Web.Mvc;

namespace ERP.Areas.HR
{
    public class HRAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HR_default",
                "HR/{controller}/{action}/{id}",
                new { controller = "Festival", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
