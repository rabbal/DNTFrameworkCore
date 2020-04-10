using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Linq;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Tasks.Models;
using Microsoft.EntityFrameworkCore;
using Task = DNTFrameworkCore.TestWebApp.Domain.Tasks.Task;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks
{
    public interface ITaskService : ICrudService<int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
    {
    }

    public class TaskService :
        EFCore.Application.CrudService<Task, int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>,
        ITaskService
    {
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork uow, IEventBus bus, IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override Task<IPagedResult<TaskReadModel>> ReadPagedListAsync(TaskFilteredPagedRequest model,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking()
                .WhereIf(model.State.HasValue, t => t.State == model.State)
                .ProjectTo<TaskReadModel>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(model, cancellationToken);
        }

        protected override void MapToEntity(TaskModel model, Task task)
        {
            _mapper.Map(model, task);
        }

        protected override TaskModel MapToModel(Task entity)
        {
            return _mapper.Map<TaskModel>(entity);
        }
    }
}