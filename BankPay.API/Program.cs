using BankPay.API.Data;
using BankPay.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BankPayApiContext>(options => options.UseInMemoryDatabase("BankPayApiDatabase"));
builder.Services.AddScoped<BankPayApiContext, BankPayApiContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

// Users ---------------------------------------------------

app.MapGet("v1/Users", async (BankPayApiContext dbContext) =>
            await dbContext.Users.Include(u => u.Account)
                                 .ToListAsync());

app.MapGet("v1/Users/{id}", async (int id, BankPayApiContext dbContext) =>
            await dbContext.Users.Include(u => u.Account)
                                 .FirstOrDefaultAsync(u => u.Id == id));


app.MapPost("v1/Users", async (User user, BankPayApiContext dbContext) =>
{
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();
    return user;
});

app.MapPut("v1/Users/{id}", async (int id, User user, BankPayApiContext dbContext) =>
{
    var data = dbContext.Users.FirstOrDefault(u => u.Id == id);

    if(data == null)
    {
        return Results.NotFound();
    }
    
    data.Name = user.Name ?? data.Name;
    data.Phone = user.Phone ?? data.Phone;
    bool isUpdated = dbContext.SaveChanges() > 0;
    if (isUpdated)
    {
        return Results.Ok("User has been modified");
    }
    return Results.BadRequest("User modified failed");

});

app.MapDelete("v1/Users/{id}", async (int id, BankPayApiContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    if (user != null)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }
    return;
});


// Accounts ---------------------------------------------------

app.MapGet("v1/Accounts", async (BankPayApiContext dbContext) =>
            await dbContext.Accounts.ToListAsync());

app.MapGet("v1/Accounts/{id}", async (int id, BankPayApiContext dbContext) =>
            await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id));


app.MapPut("v1/Accounts/AddCredit/{id}", async (int id, Account account, BankPayApiContext dbContext) =>
{
    var data = dbContext.Accounts.FirstOrDefault(u => u.Id == id);

    if (data == null)
    {
        return Results.NotFound();
    }

    if (!(data.NumberAccount == account.NumberAccount))
    {
        return Results.BadRequest("Invalid account number!");
    }

    data.AddCredit(account.Balance);
    await dbContext.SaveChangesAsync();
    return Results.Ok($"Credited amount of {account.Balance}. Total amount: {data.Balance}");
});

app.MapPut("v1/Accounts/Withdraw/{id}", async (int id, Account account, BankPayApiContext dbContext) =>
{
    var data = dbContext.Accounts.FirstOrDefault(u => u.Id == id);

    if (data == null)
    {
        return Results.NotFound();
    }

    if (!(data.NumberAccount == account.NumberAccount))
    {
        return Results.BadRequest("Invalid account number!");
    }

    data.Withdraw(account.Balance);
    await dbContext.SaveChangesAsync();
    return Results.Ok($"Debit amount of {account.Balance}. Total amount: {data.Balance}");
});


app.Run();


//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/dai", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateTime.Now.AddDays(index),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");


//internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}