using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Linq;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using Microsoft.EntityFrameworkCore;
using Task = DNTFrameworkCore.TestAPI.Domain.Tasks.Task;
namespace DNTFrameworkCore.TestAPI.Application.Tasks
{
    public interface ITaskService : IEntityService<int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
    {
        bool TamperedTaskExists();
    }

    public class TaskService : EntityService<Task, int, TaskReadModel, TaskModel,
        TaskFilteredPagedRequest>, ITaskService
    {
        public TaskService(IDbContext dbContext, IEventBus bus) : base(dbContext, bus)
        {
        }

        public bool TamperedTaskExists()
        {
            var tasks = EntitySet.ToList();
            return tasks.Any(task => EFCoreShadow.PropertyHash(task) != DbContext.EntityHash(task));
        }

        public override Task<IPagedResult<TaskReadModel>> FetchPagedListAsync(TaskFilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            request.SortingIfNullOrEmpty("Id DESC");
            return EntitySet.AsNoTracking()
                .WhereIf(request.State.HasValue, t => t.State == request.State)
                .Select(t => new TaskReadModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    State = t.State,
                    Number = t.Number,
                    LocalDateTime = t.LocalDateTime,
                    CreatedDateTime = EFCoreShadow.PropertyCreatedDateTime(t),
                    NullableDateTime = t.NullableDateTime
                }).ToPagedListAsync(request, cancellationToken);
        }

        protected override void MapToEntity(TaskModel model, Task task)
        {
            task.Title = model.Title;
            task.Number = model.Number;
            task.Description = model.Description;
            task.State = model.State;
            task.Version = model.Version;
            task.NormalizedTitle = model.Title.ToUpperInvariant();
            
            if (!task.IsTransient()) return;

            task.BranchId = 3;
            task.LocalDateTime = DateTime.UtcNow.ToLocalTime();
            task.NullableDateTime = DateTime.UtcNow;
        }

        protected override TaskModel MapToModel(Task task)
        {
            return new TaskModel
            {
                Title = task.Title,
                Number = task.Number,
                Description = task.Description,
                State = task.State,
                Version = task.Version,
                Id = task.Id
            };
        }
    }
}