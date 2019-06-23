using System;
using System.ComponentModel.DataAnnotations;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Domain.Tracking;

namespace DNTFrameworkCore.EntityFramework.Tests.Context.Hooks
{
    public class TimestampedSoftDeletedEntity : IHasCreationDateTime, IHasModificationDateTime, ISoftDeleteEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
        public DateTimeOffset? LastModificationDateTime { get; set; }
    }
    
    public class SimpleEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreationDateTime { get; set; }
        public DateTimeOffset? LastModificationDateTime { get; set; }
    }

    public class ValidatedEntity : IHasCreationDateTime, IHasModificationDateTime
    {
        public int Id { get; set; }
        [Required] public string Text { get; set; }

        public DateTimeOffset CreationDateTime { get; set; }
        public DateTimeOffset? LastModificationDateTime { get; set; }
    }
}