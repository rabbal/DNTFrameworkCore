using System;
using System.Threading.Tasks;
using DNTFrameworkCore.EFCore.Context;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.EFCore.Transaction
{
    public class TransactionFilter : IAsyncActionFilter
    {
        private readonly IDbContext _dbContext;

        public TransactionFilter(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                await _dbContext.BeginTransactionAsync();

                var actionExecuted = await next();
                if (actionExecuted.Exception != null && !actionExecuted.ExceptionHandled)
                {
                    _dbContext.RollbackTransaction();
                }
                else
                {
                    await _dbContext.CommitTransactionAsync();
                }
            }
            catch (Exception)
            {
                _dbContext.RollbackTransaction();
                throw;
            }
        }
    }
}