using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
using DNTFrameworkCore.TestWebApp.Domain.Blogging;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging
{
    public interface IBlogService : ICrudService<int, BlogModel>
    {
    }

    public class BlogService : CrudService<Blog, int, BlogModel>, IBlogService
    {
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork uow, IEventBus bus, IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper;
        }

        public override Task<IPagedResult<BlogModel>> ReadPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking()
                .Select(b => new BlogModel
                {
                    Id = b.Id,
                    Url = b.Url,
                    Title = b.Title
                }).ToPagedListAsync(request, cancellationToken);
        }

        protected override void MapToEntity(BlogModel model, Blog blog)
        {
            _mapper.Map(model, blog);
        }

        protected override BlogModel MapToModel(Blog blog)
        {
            return _mapper.Map<BlogModel>(blog);
        }
    }
}