using Amazon.SQS;

namespace TransactionalOutboxPatternApp.Infrastructure.MessageBus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services,
        Action<MessageBusOptions> queueConfigBuilder) =>
        services.AddAWSService<IAmazonSQS>()
            .AddSingleton<IMessageBus, MessageBus>()
            .AddOptions<MessageBusOptions>().Configure(queueConfigBuilder)
            .Services;
}