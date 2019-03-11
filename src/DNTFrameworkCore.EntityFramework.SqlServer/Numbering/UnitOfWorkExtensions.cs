using DNTFrameworkCore.EntityFramework.Context;

namespace DNTFrameworkCore.EntityFramework.SqlServer.Numbering
{
    public static class UnitOfWorkExtensions
    {
        public static void AcquireDistributedLock(this IUnitOfWork uow, string resource)
        {
            uow.ExecuteSqlCommand(@"EXEC sp_getapplock @Resource={0}, @LockOwner={1}, 
                        @LockMode={2} , @LockTimeout={3};", resource, "Transaction", "Exclusive", 15000);
        }
    }
}