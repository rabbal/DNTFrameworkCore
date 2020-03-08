using DNTFrameworkCore.EFCore.Context;

namespace ProjectName.IntegrationTests
{
    public static class UnitOfWorkExtensions
    {
        public static void SetRowVersionOnInsert(this IUnitOfWork uow, string table)
        {
            uow.ExecuteSqlRawCommand(
                $@"
                    CREATE TRIGGER Set{table}RowVersion
                    AFTER INSERT ON {table}
                    BEGIN
                        UPDATE {table}
                        SET Version = randomblob(8)
                        WHERE Id = NEW.Id;
                    END
                    ");
        }

        public static void SetRowVersionOnUpdate(this IUnitOfWork uow, string table)
        {
            uow.ExecuteSqlRawCommand(
                $@"
                    CREATE TRIGGER Set{table}RowVersion
                    AFTER UPDATE ON {table}
                    BEGIN
                        UPDATE {table}
                        SET Version = randomblob(8)
                        WHERE Id = NEW.Id;
                    END
                    ");
        }
    }
}