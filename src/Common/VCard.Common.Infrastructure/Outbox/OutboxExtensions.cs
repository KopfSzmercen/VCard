using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace VCard.Common.Infrastructure.Outbox;

public static class OutboxExtensions
{
    public static void ConfigureOutbox<TDbContext>(this IBusRegistrationConfigurator configurator)
        where TDbContext : DbContext
    {
        configurator.AddEntityFrameworkOutbox<TDbContext>(efCoreOutboxConfigurator =>
        {
            efCoreOutboxConfigurator.UsePostgres();

            efCoreOutboxConfigurator.UseBusOutbox();
        });
    }
}