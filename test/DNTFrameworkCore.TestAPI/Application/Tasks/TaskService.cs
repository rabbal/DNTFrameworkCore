using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Linq;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using DNTPersianUtils.Core;
using Microsoft.EntityFrameworkCore;
using Task = DNTFrameworkCore.TestAPI.Domain.Tasks.Task;

namespace DNTFrameworkCore.TestAPI.Application.Tasks
{
    public interface ITaskService : ICrudService<int, TaskReadModel, TaskModel, TaskFilteredPagedRequestModel>
    {
        bool TamperedTaskExists();
    }

    public class TaskService : EFCore.Application.CrudService<Task, int, TaskReadModel, TaskModel,
        TaskFilteredPagedRequestModel>, ITaskService
    {
        private readonly IMapper _mapper;

        public TaskService(
            IUnitOfWork uow,
            IEventBus bus,
            IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public bool TamperedTaskExists()
        {
            var tasks = EntitySet.ToList();
            return tasks.Any(task => task.Hash != UnitOfWork.EntityHash(task));
        }

        public override Task<IPagedResult<TaskReadModel>> ReadPagedListAsync(TaskFilteredPagedRequestModel model,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking()
                .WhereIf(model.State.HasValue, t => t.State == model.State)
                .Select(t => new TaskReadModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    State = t.State,
                    Number = t.Number,
                    LocalDateTime = t.LocalDateTime,
                    CreatedDateTime = t.CreatedDateTime,
                    NullableDateTime = t.NullableDateTime
                }).ToPagedListAsync(model, cancellationToken);
        }

        protected override void MapToEntity(TaskModel model, Task task)
        {
            _mapper.Map(model, task);
            if (task.IsTransient())
            {
                task.BranchId = 3;
                task.LocalDateTime = DateTime.UtcNow.ToIranTimeZoneDateTime();
                task.NullableDateTime = DateTime.UtcNow;
            }
        }

        protected override TaskModel MapToModel(Task task)
        {
            return _mapper.Map<TaskModel>(task);
        }
    }
}