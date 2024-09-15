using VCard.Common.Infrastructure;
using VCard.Communication.Api;
using VCard.Communication.Api.EmailSender;
using VCard.Communication.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

await app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();