using BP.AWS.Messaging;
using TransactionalOutboxPatternApp.Domain;
using TransactionalOutboxPatternApp.Infrastructure;

namespace TransactionalOutboxPatternApp;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        const string secretName = "bp-db-secret";
        const string queueName = "bp-tx-ob.fifo";

        builder.Configuration.AddSystemsManager(source =>
        {
            source.Path = $"/aws/reference/secretsmanager/{secretName}";
            source.Prefix = secretName;
        });

        builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(secretName));
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddMessageBus(messageBus =>
        {
            messageBus
                .SetDefaultAccountId(Environment.GetEnvironmentVariable("AWS_ACCOUNT_ID") ?? "")
                .SetDefaultRegion(Environment.GetEnvironmentVariable("AWS REGION") ?? "")
                .MapTypeToQueue<TransactionOutboxRecordsAdded>(queueName)
                .MapTypeToQueue<OrderStatusChanged>(queueName)
                .UseHandler<TransactionOutboxRecordsAddedHandler, TransactionOutboxRecordsAdded>();
        });
    }
}