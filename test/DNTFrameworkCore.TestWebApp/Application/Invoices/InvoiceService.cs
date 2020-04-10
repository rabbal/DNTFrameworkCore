using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
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

        protected override IQueryable<Invoice> FindEntityQueryable => base.FindEntityQueryable.Include(i => i.Items);

        public override Task<IPagedResult<InvoiceReadModel>> ReadPagedListAsync(FilteredPagedRequestModel model,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking().Select(i => new InvoiceReadModel
            {
                Id = i.Id,
                Number = i.Number,
                CreatedDateTime = i.CreatedDateTime
            }).ToPagedListAsync(model, cancellationToken);
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