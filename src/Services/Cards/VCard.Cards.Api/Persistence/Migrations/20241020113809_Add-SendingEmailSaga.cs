using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VCard.Cards.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSendingEmailSaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Position",
                schema: "cards",
                table: "Checkpoints",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Position",
                schema: "cards",
                table: "Checkpoints",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");
        }
    }
}
