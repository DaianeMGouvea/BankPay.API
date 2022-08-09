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
      .Produces(StatusCodes.Status404NotFound);

    app.MapPost("v1/Users", async (UserPostModel user, IUsersRepository usersRepository) =>
    {
        bool userExist = await usersRepository.UserExist(user.Cpf);
        if (userExist)
            return Results.BadRequest("User already registered!");

        var data = await usersRepository.AddUser(new User(user));
        return data > 0 ? Results.Ok("User has been created") : Results.BadRequest("Unexpected error! User created failed");

    }).Produces<Account>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

    app.MapPut("v1/Users/{id}", async (int id, UserPutModel user, IUsersRepository usersRepository) =>
    {
        if (user == null)
            return Results.NotFound();

        var data = await usersRepository.FindBy(id);
        if (data is null)
            return Results.BadRequest();

        data.applyChanges(user);

        int update = await usersRepository.Update(data);

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

    app.MapPut("v1/Accounts/{numberAccount}/AddCredit", async (int numberAccount, AccountPutModel account, IAccountsRepository accountRepository) =>
    {
        var accountValid = await accountRepository.FindByNumberAccount(numberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");

        var addCredit = await accountRepository.AddCredit(accountValid, account.Amount);
        return addCredit > 0 ? Results.Ok(accountValid.OccurrenceRecords) : Results.BadRequest("Unexpected error! Add Credits failed!");

    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);

    app.MapPut("v1/Accounts/{numberAccount}/Withdraw", async (int numberAccount, AccountPutModel account, IAccountsRepository accountRepository) =>
    {
        var accountValid = await accountRepository.FindByNumberAccount(numberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");


        var withdraw = await accountRepository.Withdraw(accountValid, account.Amount);
        return withdraw > 0 ? Results.Ok(accountValid.OccurrenceRecords) : Results.BadRequest("Unexpected error! Add Credits failed!");

    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound);


    // OccurrenceRecords ------------------------------------------------------------------------------

    app.MapGet("v1/OcurrencesRecord", async (IOccurrenceRecordRepository ocorrenceRecord) =>
    {
        var resp = await ocorrenceRecord.GetOcurrencesRecord();
        return resp is not null ? Results.Ok(resp) : Results.NoContent();
    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status204NoContent);

    app.MapGet("v1/Accounts/{numberAccount}/OcurrencesRecord/Statement", async (int numberAccount, IOccurrenceRecordRepository ocorrenceRecord) =>
    {
        var accountValid = await ocorrenceRecord.AccountValid(numberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");

        var statement = await ocorrenceRecord.Statement(accountValid.Id);

        return statement is not null ? Results.Ok(statement) : Results.NoContent();

    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status204NoContent)
      .Produces(StatusCodes.Status400BadRequest);

    app.MapGet("v1/Accounts/{numberAccount}/OccurrenceRecord/OccurrenceRecordsYear", async (int year, int numberAccount, IOccurrenceRecordRepository ocorrenceRecord) =>
    {
        var accountValid = await ocorrenceRecord.AccountValid(numberAccount);
        if (accountValid is null)
            return Results.BadRequest("Invalid account number!");

        var filterYear = await ocorrenceRecord.FilterYear(year, accountValid);
        var data = ocorrenceRecord.FilterMonth(filterYear);

        return data is not null ? Results.Ok(data) : Results.NoContent();

    }).Produces<OcurrenceRecord>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status204NoContent)
      .Produces(StatusCodes.Status400BadRequest);

}

#endregion
