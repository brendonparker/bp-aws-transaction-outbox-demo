using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BP.TransactionalOutboxDemo.Domain;
using BP.TransactionalOutboxDemo.Infrastructure;
using OrderOrNotFound =
    System.Threading.Tasks.Task<Microsoft.AspNetCore.Http.HttpResults.Results<
        Microsoft.AspNetCore.Http.HttpResults.Ok<BP.TransactionalOutboxDemo.Domain.Order>,
        Microsoft.AspNetCore.Http.HttpResults.BadRequest<BP.TransactionalOutboxDemo.ErrorResponse>,
        Microsoft.AspNetCore.Http.HttpResults.NotFound>>;

namespace BP.TransactionalOutboxDemo;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
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
    }
}