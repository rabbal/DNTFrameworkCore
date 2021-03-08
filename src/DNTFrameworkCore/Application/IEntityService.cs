using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Application
{
    public interface IEntityService<TModel> : IEntityService<int, TModel>
        where TModel : MasterModel<int>
    {
    }

    public interface IEntityService<in TKey, TModel> : IEntityService<TKey, TModel, TModel>
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
    }

    public interface
        IEntityService<in TKey, TReadModel, TModel> : IEntityService<TKey, TReadModel, TModel, FilteredPagedRequest>
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    public interface IEntityService<in TKey, TReadModel, TModel, in TFilteredPagedRequest> : IApplicationService
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedRequest : IFilteredPagedRequest
        where TKey : IEquatable<TKey>
    {
        Task<IPagedResult<TReadModel>> FetchPagedListAsync(TFilteredPagedRequest request,
            CancellationToken cancellationToken = default);

        Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TModel>> FindListAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TModel>> FindListAsync(CancellationToken cancellationToken = default);

        Task<IPagedResult<TModel>> FindPagedListAsync(IPagedRequest request,
            CancellationToken cancellationToken = default);

        Task<TModel> CreateNewAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task<Result> EditAsync(TModel model, CancellationToken cancellationToken = default);
        Task<Result> EditAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
    }
}