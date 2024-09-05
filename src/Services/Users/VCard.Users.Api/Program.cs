using FluentValidation;
using VCard.Users.Api.Auth;
using VCard.Users.Api.Persistence;
using VCard.Users.Api.Presentation;
using VCard.Users.Api.Presentation.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();

builder.Services.AddAuth();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<VCard.Users.Api.Program>(includeInternalTypes: true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddUsersEndpoint();


await app.ApplyMigrations();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();

namespace VCard.Users.Api
{
    public class Program
    {
    }
}