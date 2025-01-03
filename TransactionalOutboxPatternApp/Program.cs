using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionalOutboxPatternApp;
using TransactionalOutboxPatternApp.Domain;
using OrderOrNotFound =
    System.Threading.Tasks.Task<Microsoft.AspNetCore.Http.HttpResults.Results<
        Microsoft.AspNetCore.Http.HttpResults.Ok<TransactionalOutboxPatternApp.Domain.Order>,
        Microsoft.AspNetCore.Http.HttpResults.BadRequest<TransactionalOutboxPatternApp.ErrorResponse>,
        Microsoft.AspNetCore.Http.HttpResults.NotFound>>;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    opts.UseSqlite("Data Source=/tmp/tx_outbox_demo.sqlite3");
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.MapGet("/order/{orderId:long}",
    async OrderOrNotFound ([FromRoute] long orderId,
        [FromServices] ApplicationDbContext dbContext) =>
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null) return TypedResults.NotFound();
        return TypedResults.Ok(order);
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

using (var temp = app.Services.CreateScope())
{
    var dbContext = temp.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();