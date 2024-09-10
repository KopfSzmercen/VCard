using VCard.Common.Infrastructure;
using VCard.Communication.Api;
using VCard.Communication.Api.EmailSender;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EmailSenderConfiguration>(
    builder.Configuration.GetRequiredSection(EmailSenderConfiguration.SectionName)
);

builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddEventBusWithTransport(
    [ConsumersRegistry.ConfigureConsumers],
    "Communication",
    builder.Configuration.GetRequiredSection("RabbitMq").Get<RabbitMqConfiguration>()!
);

builder.Services.RegisterIntegrationEventsHandlers(
    AssemblyReference.Assembly
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();