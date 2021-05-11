using System.Web.Mvc;

namespace ERP.Areas.Invoice
{
    public class InvoiceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Invoice";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Invoice_default",
                "Invoice/{controller}/{action}/{id}",
                new { Controller = "Invoice", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
