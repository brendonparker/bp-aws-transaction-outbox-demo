using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TransactionalOutboxPatternApp.Infrastructure.MessageBus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        Action<MessageBusOptions> queueConfigBuilder)
    {
        services.AddAWSService<IAmazonSQS>();
        services.TryAddSingleton<IMessageBus, MessageBus>();
        services.TryAddSingleton<IMessageDispatcher, MessageDispatcher>();
        services.AddOptions<MessageBusOptions>()
            .Configure(queueConfigBuilder)
            .PostConfigure(x => x.RegisterHandlers(services));
        return services;
    }
}