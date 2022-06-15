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

    if (data == null)
    {
        return Results.NotFound();
    }

    data.Name = user.Name ?? data.Name;
    data.Phone = user.Phone ?? data.Phone;
    bool isUpdated = await dbContext.SaveChangesAsync() > 0;
    if (isUpdated)
    {
        return Results.Ok("User has been modified");
    }
    return Results.BadRequest("User modified failed");

});

app.MapDelete("v1/Users/{id}", async (int id, BankPayApiContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
    {
        return Results.NotFound();
    }

    dbContext.Users.Remove(user);
    bool IsDeleted = await dbContext.SaveChangesAsync() > 0;

    if (IsDeleted)
    {
        return Results.Ok("User has been deleted");
    }

    return Results.BadRequest("User deleted failed");
});


// Accounts ---------------------------------------------------

app.MapGet("v1/Accounts", async (BankPayApiContext dbContext) =>
            await dbContext.Accounts.ToListAsync());

app.MapGet("v1/Accounts/{id}", async (int id, BankPayApiContext dbContext) =>
            await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id));

app.MapGet("v1/Accounts/Statement/{id}", async (int id, BankPayApiContext dbContext) =>
{
    var data = await dbContext.Accounts.Include(a => a.OcurrenceRecords.OrderBy(o => o.CreatedAt))
                                       .FirstOrDefaultAsync(a => a.Id == id);
    return data.Statement().ToList();
});


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
    bool IsSaved = await dbContext.SaveChangesAsync() > 0;

    if (IsSaved)
    {
        return Results.Ok($"Credited amount of {account.Balance}. Total amount: {data.Balance}");
    }
    
    return Results.BadRequest("Unexpected error");
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
    bool IsSaved = await dbContext.SaveChangesAsync() > 0;

    if (IsSaved)
    {
        return Results.Ok($"Debit amount of {account.Balance}. Total amount: {data.Balance}");
    }

    return Results.BadRequest("Unexpected error");
});

app.Run();
