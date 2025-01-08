using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionalOutboxPatternApp;
using TransactionalOutboxPatternApp.Domain;
using TransactionalOutboxPatternApp.Infrastructure;
using TransactionalOutboxPatternApp.Infrastructure.MessageBus;
using OrderOrNotFound =
    System.Threading.Tasks.Task<Microsoft.AspNetCore.Http.HttpResults.Results<
        Microsoft.AspNetCore.Http.HttpResults.Ok<TransactionalOutboxPatternApp.Domain.Order>,
        Microsoft.AspNetCore.Http.HttpResults.BadRequest<TransactionalOutboxPatternApp.ErrorResponse>,
        Microsoft.AspNetCore.Http.HttpResults.NotFound>>;

var builder = WebApplication.CreateBuilder(args);

const string secretName = "bp-db-secret";

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
        .MapTypeToQueue<TransactionOutboxRecordsAdded>("bp-tx-ob.fifo")
        .MapTypeToQueue<OrderStatusChanged>("bp-tx-ob.fifo");
});
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.MapGet("/order/{orderId:long}",
    async OrderOrNotFound ([FromRoute] long orderId,
        [FromServices] ApplicationDbContext dbContext) =>
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null) return TypedResults.NotFound();
        return TypedResults.Ok(order);
    });

app.MapGet("/tx_outbox", async ([FromServices] ApplicationDbContext dbContext) =>
{
    return await dbContext.TransactionOutbox
        .OrderByDescending(x => x.Id)
        .Take(10)
        .ToListAsync();
});

app.MapPost("/order", async ([FromServices] ApplicationDbContext dbContext) =>
{
    var order = Order.Create();
    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync();
    return order;
});

app.MapPost("/order/{orderId:long}/submit",
    async OrderOrNotFound (long orderId, [FromServices] ApplicationDbContext dbContext) =>
    {
        var order = await dbContext.Orders.FindAsync(orderId);
        if (order == null)
            return TypedResults.NotFound();
        try
        {
            order.Submit();
        }
        catch
        {
            return TypedResults.BadRequest(ErrorResponse.AlreadySubmitted);
        }

        await dbContext.SaveChangesAsync();
        return TypedResults.Ok(order);
    });

// TODO: Uncomment this to run migrations
// using (var temp = app.Services.CreateScope())
// {
//     var dbContext = temp.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//     dbContext.Database.Migrate();
// }

app.Run();