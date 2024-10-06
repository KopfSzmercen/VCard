using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using VCard.Cards.Api;
using VCard.Cards.Api.EventStore;
using VCard.Cards.Api.Presentation;
using VCard.Common.Application.RequestContext;
using VCard.Common.Auth;
using VCard.Common.Infrastructure;
using VCard.Cards.Api.Persistence;
using VCard.Cards.Api.Projections;


[assembly: InternalsVisibleTo("VCard.Cards.Tests.Unit")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddRequestContext();

builder.Services.AddEventStore(builder.Configuration);

builder.Services.AddEventBusWithTransport(
    [ConsumersRegistry.ConfigureConsumers],
    "Communication",
    builder.Configuration.GetRequiredSection(RabbitMqConfiguration.SectionName)
        .Get<RabbitMqConfiguration>()!
);

builder.Services.RegisterIntegrationEventsHandlers(
    AssemblyReference.Assembly
);

builder.Services.AddPostgresPersistence(builder.Configuration);
builder.Services.AddProjections();
builder.Services.AddEventStoreSubscriptions();

var app = builder.Build();

//apply migrations
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

app.UseAuth();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRequestContext();

app.UseHttpsRedirection();

app.MapCardEndpoints();

await app.RunAsync();