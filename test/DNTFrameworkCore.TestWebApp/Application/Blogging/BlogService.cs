using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
using DNTFrameworkCore.TestWebApp.Domain.Blogging;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Blogging
{
    public interface IBlogService : IEntityService<int, BlogModel>
    {
    }

    public class BlogService : EntityService<Blog, int, BlogModel>, IBlogService
    {
        private readonly IMapper _mapper;

        public BlogService(IDbContext dbContext, IEventBus bus, IMapper mapper) : base(dbContext, bus)
        {
            _mapper = mapper;
        }

        public override Task<IPagedResult<BlogModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            request.SortingIfEmpty("Id DESC");
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