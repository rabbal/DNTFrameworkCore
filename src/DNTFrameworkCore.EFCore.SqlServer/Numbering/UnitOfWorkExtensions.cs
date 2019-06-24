using DNTFrameworkCore.EFCore.Context;

namespace DNTFrameworkCore.EFCore.SqlServer.Numbering
{
    public static class UnitOfWorkExtensions
    {
        public static void AcquireDistributedLock(this IUnitOfWork context, string resource)
        {
            context.ExecuteSqlCommand(@"EXEC sp_getapplock @Resource={0}, @LockOwner={1}, 
                        @LockMode={2} , @LockTimeout={3};", resource, "Transaction", "Exclusive", 15000);
        }
    }
}