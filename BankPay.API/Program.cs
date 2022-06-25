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
    await accountRepository.FindById(id));

app.MapPut("v1/Accounts/AddCredit/{id}", async (int id, Account account, IAccountsRepository accountRepository) =>
{
    var data = await accountRepository.FindById(id);

    if (data == null)
        return Results.NotFound();

    var accountValid = await accountRepository.FindByNumberAccount(account.NumberAccount);
    if (accountValid is null)
        return Results.BadRequest("Invalid account number!");

    var addCredit = await accountRepository.AddCredit(accountValid, account.Balance);
    return addCredit > 0 ? Results.Ok(accountValid) : Results.BadRequest("Unexpected error! Add Credits failed!");
});

app.MapPut("v1/Accounts/Withdraw/{id}", async (int id, Account account, IAccountsRepository accountRepository) =>
{
    var data = await accountRepository.FindById(id);

    if (data == null)
        return Results.NotFound();

    var accountValid = await accountRepository.FindByNumberAccount(account.NumberAccount);
    if (accountValid is null)
        return Results.BadRequest("Invalid account number!");


    var withdraw = await accountRepository.Withdraw(accountValid, account.Balance);
    return withdraw > 0 ? Results.Ok(accountValid) : Results.BadRequest("Unexpected error! Add Credits failed!");
});

// OcurrenceRecords ------------------------------------------------------------------------------

app.MapGet("v1/OcurrencesRecord/Statement/{id}", async (int id, int numberAccount, IOcorrenceRecordRepository ocorrenceRecord) =>
{
    if (ocorrenceRecord.FindByNumberAccount(numberAccount) is null)
       return Results.BadRequest("Invalid account number!");

    var data = await ocorrenceRecord.Statement();
        
    return data is not null ? Results.Ok(data) : Results.NoContent();
});

app.MapGet("v1/OcurrencesRecord/OcurrencesRecordYear/{id}", async (int id, int year, int numberAccount, IOcorrenceRecordRepository ocorrenceRecord) =>
{
    if (ocorrenceRecord.FindByNumberAccount(numberAccount) is null)
        return Results.BadRequest("Invalid account number!");

    var data = await ocorrenceRecord.OcurrencesRecordYear(year);

    int currentMonth = 0;
    int index = 0;

    List<OcurrenceRecordMonth> ocurrencesRecordMonth = new();

    foreach (var record in data)
    {
        if (currentMonth == 0)
        {
            currentMonth = record.CreatedAt.Month;
            ocurrencesRecordMonth.Add(new(record.CreatedAt.Month));
        }

        if (currentMonth != record.CreatedAt.Month)
        {
            currentMonth = record.CreatedAt.Month;
            ocurrencesRecordMonth.Add(new(record.CreatedAt.Month));
            index++;
        }

        ocurrencesRecordMonth[index].MonthBalance(record.Amount, record.TypeRecord);
    }

    return ocurrencesRecordMonth is not null ? Results.Ok(ocurrencesRecordMonth) : Results.NoContent();
});

app.Run();
