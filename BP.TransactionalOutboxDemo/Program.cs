using BP.TransactionalOutboxDemo;
using BP.TransactionalOutboxDemo.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplication();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();
app.MapEndpoints();

// TODO: Uncomment this to run migrations
// using (var temp = app.Services.CreateScope())
// {
//     var dbContext = temp.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//     dbContext.Database.Migrate();
// }

app.Run();