using BankPay.API.Data;
using BankPay.API.Models;
using BankPay.API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<BankPayApiContext>(options => options.UseInMemoryDatabase("BankPayApiDatabase"));
builder.Services.AddScoped<BankPayApiContext, BankPayApiContext>();
builder.Services.AddTransient<IUsersRepository, UsersRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.

var environment = app.Environment;

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(option =>
{
    option.AllowAnyHeader();
    option.AllowAnyMethod();
    option.AllowAnyOrigin();
});


// Users ---------------------------------------------------

app.MapGet("v1/Users", async (IUsersRepository usersRepository) =>
            await usersRepository.GetUsers());

app.MapGet("v1/Users/{id}", async (int id, IUsersRepository usersRepository) =>
            await usersRepository.FindBy(id));

app.MapPost("v1/Users", async (User user, IUsersRepository usersRepository) =>
{
    await usersRepository.AddUser(user);
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

app.MapGet("v1/Accounts/Statement/{id}", async (int id, int numberAccount, BankPayApiContext dbContext) =>
{
    var data = await dbContext.Accounts.Include(a => a.OcurrenceRecords.OrderBy(o => o.CreatedAt))
                                       .FirstOrDefaultAsync(a => a.Id == id);

    if (data == null)
    {
        return Results.NoContent();
    }

    if (!(data.NumberAccount == numberAccount))
    {
        return Results.BadRequest("Invalid account number!");
    }

    return Results.Ok(data.Statement().ToList());
});

app.MapGet("v1/Accounts/OcurrenceRecordYear/{id}", async (int id, int year, int numberAccount, BankPayApiContext dbContext) =>
{

    var data = await dbContext.Accounts.Include(a => a.OcurrenceRecords.OrderBy(o => o.CreatedAt))
                                       .FirstOrDefaultAsync(a => a.Id == id);

    if (data == null)
    {
        return Results.NoContent();
    }

    if (!(data.NumberAccount == numberAccount))
    {
        return Results.BadRequest("Invalid account number!");
    }

    return Results.Ok(data.OcurrenceRecordYear(year).ToList());
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
