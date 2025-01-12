using System.Buffers;
using System.Text.Json;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
        HandlerWrapper x = async (sp, envelope, ct) =>
        {
            var log = sp.GetRequiredService<ILogger<HandlerWrapper>>();
            log.LogInformation("Type: {Type} Envelope: {Payload}", envelope.Type, envelope.Payload);
            var svc = sp.GetRequiredService<THandler>();
            var message = envelope.Payload.Deserialize<TMessage>();
            return await svc.HandleAsync(message!, ct);
        };
        services.AddTransient<THandler>();
        services.AddKeyedTransient<HandlerWrapper>(typeof(TMessage).Name, (sp, key) => x);
        return services;
    }
}