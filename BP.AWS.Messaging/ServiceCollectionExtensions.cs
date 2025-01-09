using System.Text.Json;
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
        services.Configure(queueConfigBuilder);
        return services;
    }

    public static IServiceCollection AddHandler<THandler, TMessage>(this IServiceCollection services)
        where THandler : class, IHandler<TMessage>
    {
        HandlerWrapper x = async (serviceProvider, envelope, ct) =>
        {
            var svc = serviceProvider.GetRequiredService<THandler>();
            var message = envelope.Payload.Deserialize<TMessage>();
            return await svc.HandleAsync(message!, ct);
        };
        services.AddTransient<THandler>();
        services.AddKeyedTransient<HandlerWrapper>(typeof(TMessage).Name, (sp, key) => x);
        return services;
    }
}