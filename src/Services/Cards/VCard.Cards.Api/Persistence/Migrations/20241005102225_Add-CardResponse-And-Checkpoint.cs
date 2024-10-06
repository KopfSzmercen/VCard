using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VCard.Cards.Api.Projections.Migrations
{
    /// <inheritdoc />
    public partial class AddCardResponseAndCheckpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cards");

            migrationBuilder.CreateTable(
                name: "CardResponses",
                schema: "cards",
                columns: table => new
                {
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardResponses", x => x.CardId);
                });

            migrationBuilder.CreateTable(
                name: "Checkpoints",
                schema: "cards",
                columns: table => new
                {
                    SubscriptionId = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    CheckpointedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.SubscriptionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardResponses",
                schema: "cards");

            migrationBuilder.DropTable(
                name: "Checkpoints",
                schema: "cards");
        }
    }
}
