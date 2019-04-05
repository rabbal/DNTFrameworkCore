using System.Linq;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.TestWebApp.Application.Invoices.Models;
using DNTFrameworkCore.TestWebApp.Domain.Invoices;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Invoices
{
    public interface IInvoiceService : ICrudService<long, InvoiceReadModel, InvoiceModel>
    {
    }
    public class InvoiceService : CrudService<Invoice, long, InvoiceReadModel, InvoiceModel>, IInvoiceService
    {
        public InvoiceService(IUnitOfWork uow, IEventBus bus) : base(uow, bus)
        {
        }

        protected override IQueryable<Invoice> BuildFindQuery()
        {
            return base.BuildFindQuery().Include(i => i.Items);
        }

        protected override IQueryable<InvoiceReadModel> BuildReadQuery(FilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking().Select(i => new InvoiceReadModel
            {
                Id = i.Id,
                Number = i.Number,
                CreationDateTime = i.CreationDateTime
            });
        }

        protected override Invoice MapToEntity(InvoiceModel model)
        {
            var invoice = Factory<Invoice>.CreateInstance();

            invoice.Id = model.Id;
            invoice.RowVersion = model.RowVersion;
            invoice.Number = model.Number;
            invoice.Description = model.Description;
            invoice.Items = model.Items.Select(ii => new InvoiceItem
            {
                Id = ii.Id,
                TrackingState = ii.TrackingState,
                UnitPrice = ii.UnitPrice,
                UnitDiscount = ii.UnitDiscount,
                ProductId = ii.ProductId
            }).ToList();

            return invoice;
        }

        protected override InvoiceModel MapToModel(Invoice entity)
        {
            var model = Factory<InvoiceModel>.CreateInstance();

            model.Id = entity.Id;
            model.RowVersion = entity.RowVersion;
            model.Number = entity.Number;
            model.Description = entity.Description;
            model.Items = entity.Items.Select(ii => new InvoiceItemModel
            {
                Id = ii.Id,
                TrackingState = ii.TrackingState,
                UnitPrice = ii.UnitPrice,
                UnitDiscount = ii.UnitDiscount,
                ProductId = ii.ProductId
            }).ToList();

            return model;
        }
    }
}