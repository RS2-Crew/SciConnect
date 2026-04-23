using EventBus.Messages.Entities;
using MassTransit;

namespace AnalyticsService.Consumers
{
    public class EmployeeCreatedConsumer : IConsumer<EmployeeCreatedEvent>
    {
        private readonly ILogger<EmployeeCreatedConsumer> _logger;

        public EmployeeCreatedConsumer(ILogger<EmployeeCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Analytics: New employee created - {FirstName} {LastName} at {Institution}",
                message.FirstName, message.LastName, message.InstitutionName);

            return Task.CompletedTask;
        }
    }
}
