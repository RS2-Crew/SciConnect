using EventBus.Messages.Entities;
using MassTransit;

namespace AnalyticsService.Consumers
{
    public class InstitutionCreatedConsumer : IConsumer<InstitutionCreatedEvent>
    {
        private readonly ILogger<InstitutionCreatedConsumer> _logger;

        public InstitutionCreatedConsumer(ILogger<InstitutionCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<InstitutionCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Analytics: New institution created - {Name} ({City}, {Country})",
                message.Name, message.City, message.Country);

            return Task.CompletedTask;
        }
    }
}
