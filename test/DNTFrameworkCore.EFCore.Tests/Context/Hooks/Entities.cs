using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.EFCore.Tests.Context.Hooks
{
    public class TrackingDeletedEntity : ICreationTracking, IModificationTracking, IDeletedEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
    
    public class SimpleEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class TrackingEntity : ICreationTracking, IModificationTracking
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}