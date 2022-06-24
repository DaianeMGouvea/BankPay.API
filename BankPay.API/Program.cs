using BankPay.API.Data;
using BankPay.API.Models;
using BankPay.API.Repositories.AccountRepository;
using BankPay.API.Repositories.OcurrenceRecordRepository;
using BankPay.API.Repositories.UsersRepository;
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
builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();
builder.Services.AddTransient<IOcorrenceRecordRepository, OcorrenceRecordRepository>();


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
{
    var users = await usersRepository.GetUsers();
    return users is not null ? Results.Ok(users) : Results.NotFound();
});


app.MapGet("v1/Users/{id}", async (int id, IUsersRepository usersRepository) =>
{
    var user = await usersRepository.FindBy(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("v1/Users", async (User user, IUsersRepository usersRepository) =>
{
    await usersRepository.AddUser(user);
    return user;
});

app.MapPut("v1/Users/{id}", async (int id, User user, IUsersRepository usersRepository) =>
{
    if (user == null)
        return Results.NotFound();

    var data = usersRepository.Update(user);

    int update = await usersRepository.Update(user);

    return update > 0 ? Results.Ok("User has been modified") : Results.BadRequest("Unexpected error! User modified failed");

});

app.MapDelete("v1/Users/{id}", async (int id, IUsersRepository usersRepository) =>
{
    var user = await usersRepository.FindBy(id);

    if (user == null)
    {
        return Results.NotFound();
    }

    var Deleted = usersRepository.Delete(user);

    return await Deleted > 0 ? Results.Ok("User has been deleted") : Results.BadRequest("Unexpected error! User deleted failed");
});


// Accounts ---------------------------------------------------

app.MapGet("v1/Accounts", async (IAccountsRepository accountRepository) =>
{
    var account = await accountRepository.GetAccounts();
    return account is not null ? Results.Ok(account) : Results.NotFound();
});

app.MapGet("v1/Accounts/{id}", async (int id, IAccountsRepository accountRepository) =>
            await accountRepository.FindBy(id));

app.MapPut("v1/Accounts/AddCredit/{id}", async (int id, Account account, IAccountsRepository accountRepository) =>
{
    var data = await accountRepository.FindBy(id);

    if (data == null)
        return Results.NotFound();

    var accountValid = await accountRepository.CheckNumberAccount(account.NumberAccount);
    if (accountValid is null)
        return Results.BadRequest("Invalid account number!");

    var addCredit = await accountRepository.AddCredit(accountValid, account.Balance);
    return addCredit > 0 ? Results.Ok(accountValid) : Results.BadRequest("Unexpected error! Add Credits failed!");
});

app.MapPut("v1/Accounts/Withdraw/{id}", async (int id, Account account, IAccountsRepository accountRepository) =>
{
    var data = await accountRepository.FindBy(id);

    if (data == null)
        return Results.NotFound();

    var accountValid = await accountRepository.CheckNumberAccount(account.NumberAccount);
    if (accountValid is null)
        return Results.BadRequest("Invalid account number!");


    var withdraw = await accountRepository.Withdraw(accountValid, account.Balance);
    return withdraw > 0 ? Results.Ok(accountValid) : Results.BadRequest("Unexpected error! Add Credits failed!");
});

// OcurrenceRecords ------------------------------------------------------------------------------

app.MapGet("v1/OcurrencesRecord/Statement/{id}", async (int id, int numberAccount, IOcorrenceRecordRepository ocorrenceRecord) =>
{
    var data = ocorrenceRecord.Statement();

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

app.MapGet("v1/OcurrencesRecord/OcurrencesRecordYear/{id}", async (int id, int year, int numberAccount, BankPayApiContext dbContext) =>
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

app.Run();
