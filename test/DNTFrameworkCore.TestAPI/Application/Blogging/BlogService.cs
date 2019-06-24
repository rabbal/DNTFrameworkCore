using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestAPI.Application.Blogging.Models;
using DNTFrameworkCore.TestAPI.Domain.Blogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.TestAPI.Application.Blogging
{
    public interface IBlogService : ICrudService<int, BlogModel>
    {
    }

    public class BlogService : CrudService<Blog, int, BlogModel>, IBlogService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<BlogService> _logger;

        public BlogService(
            IUnitOfWork uow,
            IEventBus bus,
            IMapper mapper,
            ILogger<BlogService> logger) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IQueryable<BlogModel> BuildReadQuery(FilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking()
                .Select(b => new BlogModel
                {
                    Id = b.Id,
                    RowVersion = b.RowVersion,
                    Url = b.Url,
                    Title = b.Title
                });
        }

        protected override void MapToEntity(BlogModel model, Blog blog)
        {
            _mapper.Map(model, blog);
        }

        protected override BlogModel MapToModel(Blog blog)
        {
            return _mapper.Map<BlogModel>(blog);
        }

        protected override Task AfterFindAsync(IReadOnlyList<BlogModel> models)
        {
            _logger.LogInformation(nameof(AfterFindAsync));

            return Task.CompletedTask;
        }

        protected override Task AfterMappingAsync(IReadOnlyList<BlogModel> models, IReadOnlyList<Blog> blogs)
        {
            _logger.LogInformation(nameof(AfterMappingAsync));

            return Task.CompletedTask;
        }

        protected override Task<Result> BeforeCreateAsync(IReadOnlyList<BlogModel> models)
        {
            _logger.LogInformation(nameof(BeforeCreateAsync));

            return Task.FromResult(Ok());
        }

        protected override Task<Result> AfterCreateAsync(IReadOnlyList<BlogModel> models)
        {
            _logger.LogInformation(nameof(AfterCreateAsync));

            return Task.FromResult(Ok());
        }

        protected override Task<Result> BeforeEditAsync(
            IReadOnlyList<ModifiedModel<BlogModel>> models, IReadOnlyList<Blog> blogs)
        {
            _logger.LogInformation(nameof(BeforeEditAsync));

            return Task.FromResult(Ok());
        }

        protected override Task<Result> AfterEditAsync(
            IReadOnlyList<ModifiedModel<BlogModel>> models, IReadOnlyList<Blog> blogs)
        {
            _logger.LogInformation(nameof(AfterEditAsync));

            return Task.FromResult(Ok());
        }

        protected override Task<Result> BeforeDeleteAsync(IReadOnlyList<BlogModel> models)
        {
            _logger.LogInformation(nameof(BeforeDeleteAsync));

            return Task.FromResult(Ok());
        }

        protected override Task<Result> AfterDeleteAsync(IReadOnlyList<BlogModel> models)
        {
            _logger.LogInformation(nameof(AfterDeleteAsync));

            return Task.FromResult(Ok());
        }
    }
}