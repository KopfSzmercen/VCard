using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VCard.Cards.Api.Cards.GettingCard;

namespace VCard.Cards.Api.Projections.Configurations;

internal sealed class CardResponseConfiguration : IEntityTypeConfiguration<CardResponse>
{
    public void Configure(EntityTypeBuilder<CardResponse> builder)
    {
        builder.ToTable("CardResponses");

        builder.HasKey(x => x.CardId);

        builder.Property(x => x.CardId)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .ValueGeneratedNever();
    }
}