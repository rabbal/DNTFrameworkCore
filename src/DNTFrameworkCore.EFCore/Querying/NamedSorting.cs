using System.Linq;

namespace DNTFrameworkCore.EFCore.Querying
{
    //TODO: Use in ToPagedListAsync method
    public abstract class NamedSorting<TModel>
    {
        public string Name { get; set; }

        public IOrderedQueryable<TModel> Apply(IQueryable<TModel> queryable, bool descending)
        {
            return queryable is IOrderedQueryable<TModel> ? ThenBy(queryable, descending) : By(queryable, descending);
        }

        protected abstract IOrderedQueryable<TModel> By(IQueryable<TModel> queryable, bool descending);
        protected abstract IOrderedQueryable<TModel> ThenBy(IQueryable<TModel> queryable, bool descending);
    }
}