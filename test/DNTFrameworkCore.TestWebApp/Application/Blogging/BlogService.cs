using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
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
        public BlogService(IUnitOfWork uow, IEventBus bus) : base(uow, bus)
        {
        }

        public override Task<IPagedResult<BlogModel>> ReadPagedListAsync(FilteredPagedRequestModel model,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking()
                .Select(b => new BlogModel
                {
                    Id = b.Id, Version = b.Version, Url = b.Url, Title = b.Title
                }).ToPagedListAsync(model, cancellationToken);
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