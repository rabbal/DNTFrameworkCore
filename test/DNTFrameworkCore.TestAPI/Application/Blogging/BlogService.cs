using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestAPI.Application.Blogging.Models;
using DNTFrameworkCore.TestAPI.Domain.Blogging;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Application.Blogging
{
    public interface IBlogService : IEntityService<BlogModel>
    {
    }

    public class BlogService : EntityService<Blog, BlogModel>, IBlogService
    {
        public BlogService(IDbContext dbContext, IEventBus bus) : base(dbContext, bus)
        {
        }

        public override Task<BlogModel> CreateNewAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BlogModel
            {
                Title = "Some default value from somewhere"
            });
        }

        public override Task<IPagedResult<BlogModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            request.SortingIfNullOrEmpty("Id DESC");
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
            blog.Title = model.Title;
            blog.Url = model.Url;
            blog.Version = model.Version;
            blog.NormalizedTitle = model.Title.ToUpperInvariant();
        }

        protected override BlogModel MapToModel(Blog blog)
        {
            return new BlogModel
            {
                Title = blog.Title,
                Url = blog.Url,
                Version = blog.Version,
                Id = blog.Id
            };
        }
    }
}