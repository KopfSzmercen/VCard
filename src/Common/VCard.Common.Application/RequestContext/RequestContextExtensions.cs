﻿namespace VCard.Common.Application.RequestContext;

public static class RequestContextExtensions
{
    public static IServiceCollection AddRequestContext(this IServiceCollection services)
    {
        services.AddSingleton<RequestContextAccessor>();
        services.AddScoped<IRequestContext>(provider => provider.GetRequiredService<RequestContextAccessor>().Context!);
        return services;
    }

    public static IApplicationBuilder UseRequestContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            var requestContextAccessor = ctx.RequestServices.GetRequiredService<RequestContextAccessor>();
            requestContextAccessor.Context = new RequestContext(ctx);
            return next();
        });
        return app;
    }
}