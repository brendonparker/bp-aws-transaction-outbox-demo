using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BP.AWS.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        Action<MessageBusOptions> queueConfigBuilder)
    {
        services.AddAWSService<IAmazonSQS>();
        services.TryAddSingleton<IMessageBus, MessageBus>();
        services.TryAddSingleton<IHandlerInvoker, HandlerInvoker>();

        MessageBusOptions opts = new();
        queueConfigBuilder(opts);
        opts.RegisterHandlers(services);
        services.Configure(queueConfigBuilder);

        return services;
    }
}