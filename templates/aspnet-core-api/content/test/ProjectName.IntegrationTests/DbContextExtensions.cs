using DNTFrameworkCore.EFCore.Context;

namespace ProjectName.IntegrationTests
{
    public static class DbContextExtensions
    {
        public static void SetRowVersionOnInsert(this IDbContext dbContext, string table)
        {
            dbContext.ExecuteSqlRawCommand(
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

        public static void SetRowVersionOnUpdate(this IDbContext dbContext, string table)
        {
            dbContext.ExecuteSqlRawCommand(
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