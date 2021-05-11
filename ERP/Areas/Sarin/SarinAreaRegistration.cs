using System.Web.Mvc;

namespace ERP.Areas.Sarin
{
    public class SarinAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sarin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Sarin_default",
                "Sarin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
