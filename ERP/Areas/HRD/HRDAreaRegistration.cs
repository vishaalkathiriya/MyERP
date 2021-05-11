using System.Web.Mvc;

namespace ERP.Areas.HRD
{
    public class HRDAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HRD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HRD_default",
                "HRD/{controller}/{action}/{id}",
                new { controller = "HRDDDInPressMedia", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
