using System.Web.Mvc;

namespace ERP.Areas.AR
{
    public class ARAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AR_default",
                "AR/{controller}/{action}/{id}",
                new { controller = "ARModule", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
