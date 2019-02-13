using System;
using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.EntityFramework.Logging
{
    //todo: under development 
    public class ActivityLog : CreationTrackingEntity<Guid>
    {
        public string Message { get; set; }
        public long UserId { get; set; }
        public string Type { get; set; }
    }
}