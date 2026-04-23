using EventBus.Messages.Entities;
using MassTransit;

namespace AnalyticsService.Consumers
{
    public class SimpleEntityCreatedConsumer : IConsumer<SimpleEntityCreatedEvent>
    {
        private readonly ILogger<SimpleEntityCreatedConsumer> _logger;

        public SimpleEntityCreatedConsumer(ILogger<SimpleEntityCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SimpleEntityCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Analytics: New {EntityType} created - {Name}",
                message.EntityType, message.Name);

            return Task.CompletedTask;
        }
    }
}
