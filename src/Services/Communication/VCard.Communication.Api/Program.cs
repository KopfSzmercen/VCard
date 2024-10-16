using System.Runtime.CompilerServices;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;
using VCard.Common.Application.RequestContext;
using VCard.Common.Auth;
using VCard.Common.Infrastructure;
using VCard.Communication.Api;
using VCard.Communication.Api.EmailSender;
using VCard.Communication.Api.Persistence;
using VCard.Communication.Api.Presentation;

[assembly: InternalsVisibleTo("VCard.Communication.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRequestContext();

builder.Services.Configure<EmailSenderConfiguration>(
    builder.Configuration.GetRequiredSection(EmailSenderConfiguration.SectionName)
);

builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddEventBusWithTransport<AppDbContext>(
    [ConsumersRegistry.ConfigureConsumers],
    "Communication",
    builder.Configuration.GetRequiredSection("RabbitMq").Get<RabbitMqConfiguration>()!,
    true
);

builder.Services.RegisterIntegrationEventsHandlers(
    AssemblyReference.Assembly
);

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddServiceDiscovery(o => o.UseConsul());

builder.Services
    .AddHttpClient<SendEmailEndpoint.Client>(client => client.BaseAddress = new Uri("http://users-api"))
    .AddServiceDiscovery();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SendEmails", policy => policy.RequireClaim("SendEmails", true.ToString()));

var app = builder.Build();

await app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuth();
app.UseRequestContext();
app.UseHttpsRedirection();

app.MapEmailEndpoints();


await app.RunAsync();

public partial class Program;