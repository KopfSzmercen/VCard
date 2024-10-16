using FluentValidation;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;
using VCard.Common.Application.RequestContext;
using VCard.Common.Infrastructure;
using VCard.Users.Api;
using VCard.Users.Api.Auth;
using VCard.Users.Api.Persistence;
using VCard.Users.Api.Presentation;
using VCard.Users.Api.Presentation.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();

builder.Services.AddAuth();

builder.Services.AddRequestContext();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<VCard.Users.Api.Program>(includeInternalTypes: true);

builder.Services.AddEventBusWithTransport<AppDbContext>(
    [ConsumersRegistry.ConfigureConsumers],
    "Users",
    builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>()!,
    true
);

builder.Services.AddServiceDiscovery(o => o.UseConsul());

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
app.UseRequestContext();

await app.RunAsync();

namespace VCard.Users.Api
{
    public class Program
    {
    }
}