using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Linq;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using DNTFrameworkCore.TestAPI.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Application.Tasks
{
    public interface ITaskService : ICrudService<int, TaskReadModel, TaskModel, TaskFilteredPagedQueryModel>
    {
    }

    public class TaskService : CrudService<Task, int, TaskReadModel, TaskModel, TaskFilteredPagedQueryModel>,
        ITaskService
    {
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork uow, IEventBus bus, IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override IQueryable<TaskReadModel> BuildReadQuery(TaskFilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking()
                .WhereIf(model.State.HasValue, t => t.State == model.State)
                .Select(t => new TaskReadModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    State = t.State,
                    Number = t.Number
                });
        }

        protected override void MapToEntity(TaskModel model, Task task)
        {
            _mapper.Map(model, task);
        }

        protected override TaskModel MapToModel(Task task)
        {
            return _mapper.Map<TaskModel>(task);
        }
    }
}