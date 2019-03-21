using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Events;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestWebApp.Application.Tasks.Models;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks.Handlers
{
    public class TaskEditingBusinessEventHandler : IBusinessEventHandler<EditingBusinessEvent<TaskModel, int>>
    {
        private readonly ILogger<TaskEditingBusinessEventHandler> _logger;

        public TaskEditingBusinessEventHandler(ILogger<TaskEditingBusinessEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<Result> Handle(EditingBusinessEvent<TaskModel, int> @event)
        {
            foreach (var model in @event.Models)
            {
                _logger.LogInformation($"Title changed from: {model.OriginalValue.Title} to: {model.NewValue.Title}");
            }

            return Task.FromResult(Result.Ok());
        }
    }
}