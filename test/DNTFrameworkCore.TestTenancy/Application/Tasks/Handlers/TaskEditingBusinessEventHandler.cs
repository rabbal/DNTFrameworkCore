using DNTFrameworkCore.TestTenancy.Application.Tasks.Models;

namespace DNTFrameworkCore.TestTenancy.Application.Tasks.Handlers
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