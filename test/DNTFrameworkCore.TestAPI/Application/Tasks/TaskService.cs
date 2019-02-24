using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
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

        public TaskService(CrudServiceDependency dependency, IMapper mapper) : base(dependency)
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