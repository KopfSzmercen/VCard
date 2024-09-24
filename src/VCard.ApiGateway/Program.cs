using VCard.Common.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ManageCards", policy => policy.RequireClaim("ManageCards", true.ToString()));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseAuth();
app.MapReverseProxy();

await app.RunAsync();