using System;
using System.Linq;
using AutoMapper;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;
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
        private readonly IMapper _mapper;

        public InvoiceService(
            IUnitOfWork uow,
            IEventBus bus,
            IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                CreationDateTime = EF.Property<DateTimeOffset>(i, EFCore.Context.EFCore.CreationDateTime)
            });
        }

        protected override void MapToEntity(InvoiceModel model, Invoice invoice)
        {
            _mapper.Map(model, invoice);
        }

        protected override InvoiceModel MapToModel(Invoice invoice)
        {
            return _mapper.Map<InvoiceModel>(invoice);
        }
    }
}