using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Events;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Handlers
{
    public class TaskEditingDomainEventHandler : IDomainEventHandler<EditingDomainEvent<TaskModel, int>>
    {
        private readonly ILogger<TaskEditingDomainEventHandler> _logger;

        public TaskEditingDomainEventHandler(ILogger<TaskEditingDomainEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<Result> Handle(EditingDomainEvent<TaskModel, int> domainEvent)
        {
            foreach (var model in domainEvent.Models)
            {
                _logger.LogInformation($"Title changed from: {model.OriginalValue.Title} to: {model.NewValue.Title}");
            }

            return Task.FromResult(Result.Ok());
        }
    }
}