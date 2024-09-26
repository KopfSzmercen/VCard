using Microsoft.AspNetCore.Mvc;
using VCard.Common.Application.RequestContext;
using VCard.Common.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddRequestContext();

var app = builder.Build();

app.UseAuth();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRequestContext();

app.UseHttpsRedirection();

app.MapGet("/cards", (
        [FromServices] IRequestContext requestContext
    ) =>
    {
        var card = new
        {
            Id = 1,
            UserId = requestContext.Id
        };

        return card;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

await app.RunAsync();