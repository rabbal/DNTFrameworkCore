using DNTFrameworkCore.TestWebApp.Application.Invoices;
using DNTFrameworkCore.TestWebApp.Application.Invoices.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.Web.Mvc;

namespace DNTFrameworkCore.TestWebApp
{
    public class InvoicesController : CrudController<IInvoiceService, long, InvoiceReadModel, InvoiceModel>
    {
        public InvoicesController(IInvoiceService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Invoices_Create;
        protected override string EditPermissionName => PermissionNames.Invoices_Edit;
        protected override string ViewPermissionName => PermissionNames.Invoices_View;
        protected override string DeletePermissionName => PermissionNames.Invoices_Delete;
        protected override string ViewName => "_InvoiceModal";
    }
}
