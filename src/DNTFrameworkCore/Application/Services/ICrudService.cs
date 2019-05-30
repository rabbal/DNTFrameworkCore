using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Application.Services
{
    /// <summary>
    /// Contract for CrudServices
    /// </summary>
    /// <typeparam name="TModel">Model for View, Create and Edit</typeparam>
    /// <typeparam name="TKey">type of PrimaryKey </typeparam>
    public interface ICrudService<in TKey, TModel> : ICrudService<TKey, TModel, TModel>
        where TModel : MasterModel<TKey> where TKey : IEquatable<TKey>
    {
    }

    /// <summary>
    /// Contract for CrudServices
    /// </summary>
    /// <typeparam name="TReadModel">Model for View</typeparam>
    /// <typeparam name="TModel">Model for Create and Edit</typeparam>
    /// <typeparam name="TKey">type of PrimaryKey </typeparam>
    public interface
        ICrudService<in TKey, TReadModel, TModel> : ICrudService<TKey, TReadModel, TModel, FilteredPagedQueryModel>
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    /// <summary>
    /// Contract for CrudServices
    /// </summary>
    /// <typeparam name="TReadModel">Model for View</typeparam>
    /// <typeparam name="TModel">Model for Create and Edit</typeparam>
    /// <typeparam name="TFilteredPagedQueryModel">Model for Request</typeparam>
    /// <typeparam name="TKey">type of PrimaryKey </typeparam>
    public interface ICrudService<in TKey, TReadModel, TModel, in TFilteredPagedQueryModel> : IApplicationService
        where TModel : MasterModel<TKey>
        where TReadModel : ReadModel<TKey>
        where TFilteredPagedQueryModel : IFilteredPagedQueryModel
        where TKey : IEquatable<TKey>
    {
        Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel model);
        Task<Maybe<TModel>> FindAsync(TKey id);
        Task<IReadOnlyList<TModel>> FindListAsync(IEnumerable<TKey> ids);
        Task<IReadOnlyList<TModel>> FindListAsync();
        Task<IPagedQueryResult<TModel>> FindPagedListAsync(PagedQueryModel model);
        Task<bool> ExistsAsync(TKey id);
        Task<Result> CreateAsync(TModel model);
        Task<Result> CreateAsync(IEnumerable<TModel> models);
        Task<Result> EditAsync(TModel model);
        Task<Result> EditAsync(IEnumerable<TModel> models);
        Task<Result> DeleteAsync(TModel model);
        Task<Result> DeleteAsync(IEnumerable<TModel> models);
        Task<Result> DeleteAsync(TKey id);
        Task<Result> DeleteAsync(IEnumerable<TKey> ids);
    }
}