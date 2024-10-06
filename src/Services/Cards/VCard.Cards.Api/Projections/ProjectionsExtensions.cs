using Microsoft.EntityFrameworkCore;
using VCard.Cards.Api.Cards.Creating;
using VCard.Cards.Api.Cards.GettingCard;
using VCard.Cards.Api.Cards.MoneyAdding;
using VCard.Cards.Api.Cards.MoneyWithdrawal;
using VCard.Cards.Api.Events;
using VCard.Cards.Api.Persistence;

namespace VCard.Cards.Api.Projections;

internal static class ProjectionsExtensions
{
    public static IServiceCollection AddProjections(this IServiceCollection services)
    {
        services.AddScoped<IEventHandler<EventEnvelope<CardCreated>>>(sp =>
        {
            var dbContext = sp.GetRequiredService<AppDbContext>();
            return new AddProjection<CardResponse, CardCreated, AppDbContext>(
                dbContext,
                envelope => CardResponseProjection.Handle(envelope.Data)
            );
        });

        services.AddScoped<IEventHandler<EventEnvelope<MoneyAdded>>>(sp =>
        {
            var dbContext = sp.GetRequiredService<AppDbContext>();
            return new UpdateProjection<CardResponse, MoneyAdded, AppDbContext>(
                dbContext,
                envelope => envelope.CardId,
                (envelope, view) => CardResponseProjection.Handle(envelope.Data, view)
            );
        });

        services.AddScoped<IEventHandler<EventEnvelope<MoneyWithdrawn>>>(sp =>
        {
            var dbContext = sp.GetRequiredService<AppDbContext>();
            return new UpdateProjection<CardResponse, MoneyWithdrawn, AppDbContext>(
                dbContext,
                envelope => envelope.CardId,
                (envelope, view) => CardResponseProjection.Handle(envelope.Data, view)
            );
        });

        return services;
    }
}