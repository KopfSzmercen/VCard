using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace VCard.Common.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtTokensOptions>? configureOptions = null)
    {
        var jwtConfiguration = configuration
            .GetSection(JwtTokensOptions.SectionName)
            .Get<JwtTokensOptions>();

        var section = configuration
            .GetSection(JwtTokensOptions.SectionName);

        Console.WriteLine("JWT CONFIGURATION SECTION ");
        Console.WriteLine(section);

        services.Configure<JwtTokensOptions>(configuration.GetSection(JwtTokensOptions.SectionName));

        configureOptions?.Invoke(jwtConfiguration);

        services.AddAuthorization();

        Console.WriteLine("JWT CONFIGURATION");
        Console.WriteLine("SigningKey: " + jwtConfiguration.SigningKey);
        Console.WriteLine("Audience: " + jwtConfiguration.Audience);
        Console.WriteLine("Issuer: " + jwtConfiguration.Issuer);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfiguration.SigningKey)),
                    ValidAudience = jwtConfiguration.Audience,
                    ValidIssuer = jwtConfiguration.Issuer
                };
            });

        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}