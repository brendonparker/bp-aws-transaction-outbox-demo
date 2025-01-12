using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using BP.AWS.Messaging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TransactionOutboxPatternApp;

public class QueueLambda
{
    private readonly IHandlerInvoker _handlerInvoker;

    public QueueLambda()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.AddApplication();
        var host = builder.Build();
        _handlerInvoker = host.Services.GetRequiredService<IHandlerInvoker>();
    }

    public async Task Handler(SQSEvent sqsEvent, ILambdaContext lambdaContext)
    {
        using var cts = new CancellationTokenSource(lambdaContext.RemainingTime);
        foreach (var record in sqsEvent.Records)
        {
            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(record.Body);
            await _handlerInvoker.InvokeAsync(envelope!, cts.Token);
        }
    }
}