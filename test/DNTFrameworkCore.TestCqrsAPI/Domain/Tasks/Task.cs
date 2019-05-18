using DNTFrameworkCore.Domain.Entities.Tracking;
using System;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Tasks
{
    public class Task : TrackableEntity<Guid>
    {
        private string _title;
        public TaskTitle Title
        {
            get => (TaskTitle)_title;
            set => _title = value;
        }
        public TaskState State { get; set; }
    }
}
