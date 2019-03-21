using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Linq;
using DNTFrameworkCore.TestWebApp.Application.Tasks.Models;
using DNTFrameworkCore.TestWebApp.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks
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
                .ProjectTo<TaskReadModel>(_mapper.ConfigurationProvider);
        }

        protected override Task MapToEntity(TaskModel model)
        {
            return _mapper.Map<Task>(model);
        }

        protected override TaskModel MapToModel(Task entity)
        {
            return _mapper.Map<TaskModel>(entity);
        }
    }
}