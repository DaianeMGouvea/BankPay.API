using BankPay.API.Data;
using BankPay.API.Models;
using BankPay.API.Repositories.AccountRepository;
using BankPay.API.Repositories.OccurrenceRecordRepository;
using BankPay.API.Repositories.UsersRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<BankPayApiContext>(options => options.UseInMemoryDatabase("BankPayApiDatabase"));
builder.Services.AddScoped<BankPayApiContext, BankPayApiContext>();
builder.Services.AddTransient<IUsersRepository, UsersRepository>();
builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();
builder.Services.AddTransient<IOccurrenceRecordRepository, OccurrenceRecordRepository>();


var app = builder.Build();

#endregion

#region Configure Pipilane

var environment = app.Environment;

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "BankPayAPI V1");
});

app.UseCors(option =>
{
    option.AllowAnyHeader();
    option.AllowAnyMethod();
    option.AllowAnyOrigin();
});

MapActions(app);

app.Run();

#endregion

#region EndPoints

void MapActions(WebApplication app)
{
    // Users ---------------------------------------------------

    app.MapGet("v1/Users", async (IUsersRepository usersRepository) =>
    {
        var users = await usersRepository.GetUsers();
        return users is not null ? Results.Ok(users) : Results.NotFound();

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);


    app.MapGet("v1/Users/{id}", async (int id, IUsersRepository usersRepository) =>
    {
        var user = await usersRepository.FindBy(id);
        return user is not null ? Results.Ok(user) : Results.NotFound();

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound); ;

    app.MapPost("v1/Users", async (User user, IUsersRepository usersRepository) =>
    {
        //bool userExist = await usersRepository.UserExist(user);
        //if (userExist)
        //    return Results.BadRequest("User already registered!");

        var data = await usersRepository.AddUser(user);
        return data is not null ? Results.Ok(data) : Results.NotFound();

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

    app.MapPut("v1/Users/{id}", async (int id, User user, IUsersRepository usersRepository) =>
    {
        if (user == null)
            return Results.NotFound();

        int update = await usersRepository.Update(user);

        return update > 0 ? Results.Ok("User has been modified") : Results.BadRequest("Unexpected error! User modified failed");

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces<Account>(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);

    app.MapDelete("v1/Users/{id}", async (int id, IUsersRepository usersRepository) =>
    {
        var user = await usersRepository.FindBy(id);

        if (user == null)
            return Results.NotFound();

        var Deleted = usersRepository.Delete(user);

        return await Deleted > 0 ? Results.Ok("User has been deleted") : Results.BadRequest("Unexpected error! User deleted failed");

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces<Account>(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);


    // Accounts ---------------------------------------------------

    app.MapGet("v1/Accounts", async (IAccountsRepository accountRepository) =>
    {
        var account = await accountRepository.GetAccounts();
        return account is not null ? Results.Ok(account) : Results.NotFound();

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

    app.MapGet("v1/Accounts/{id}", async (int id, IAccountsRepository accountRepository) =>
    {
        var account = await accountRepository.FindById(id);
        return account is not null ? Results.Ok(account) : Results.NotFound();

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

    app.MapPut("v1/Accounts/AddCredit/{id}", async (int id, Account account, IAccountsRepository accountRepository) =>
    {
        var data = await accountRepository.FindById(id);
        if (data == null)
            return Results.NotFound();

        var accountValid = await accountRepository.AccountValid(data.Id, account.NumberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");

        var addCredit = await accountRepository.AddCredit(accountValid, account.Balance);
        return addCredit > 0 ? Results.Ok(accountValid) : Results.BadRequest("Unexpected error! Add Credits failed!");

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);

    app.MapPut("v1/Accounts/Withdraw/{id}", async (int id, Account account, IAccountsRepository accountRepository) =>
    {
        var data = await accountRepository.FindById(id);
        if (data == null)
            return Results.NotFound();

        var accountValid = await accountRepository.AccountValid(data.Id, account.NumberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");


        var withdraw = await accountRepository.Withdraw(accountValid, account.Balance);
        return withdraw > 0 ? Results.Ok(accountValid) : Results.BadRequest("Unexpected error! Add Credits failed!");

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);


    // OccurrenceRecords ------------------------------------------------------------------------------

    app.MapGet("v1/Accounts/{idAccount}/OcurrencesRecord/Statement", async (int idAccount, int numberAccount, IOccurrenceRecordRepository ocorrenceRecord) =>
    {
        var account = await ocorrenceRecord.FindAccountById(idAccount);
        if (account == null)
            return Results.NotFound();

        var accountValid = await ocorrenceRecord.AccountValid(account.Id, numberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");

        var statement = await ocorrenceRecord.Statement(idAccount);

        return statement is not null ? Results.Ok(statement) : Results.NoContent();

    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status204NoContent)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);


    app.MapGet("v1/Accounts/{idAccount}/OccurrenceRecord/OccurrenceRecordsYear", async (int idAccount, int year, int numberAccount, IOccurrenceRecordRepository ocorrenceRecord) =>
    {
        var account = await ocorrenceRecord.FindAccountById(idAccount);
        if (account == null)
            return Results.NotFound();

        var accountValid = await ocorrenceRecord.AccountValid(account.Id, numberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");

        var filterYear = await ocorrenceRecord.FilterYear(year, account);
        var data = ocorrenceRecord.FilterMonth(filterYear);

        return data is not null ? Results.Ok(data.ElementAt(0)) : Results.NoContent();

    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status204NoContent)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);

}
#endregion
