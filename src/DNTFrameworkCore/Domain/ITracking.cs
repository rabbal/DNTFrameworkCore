namespace DNTFrameworkCore.Domain
{
    public interface ITracking : ICreationTracking, IModificationTracking
    {
    }

    public interface IModificationTracking
    {
    }

    public interface ICreationTracking
    {
    }
}