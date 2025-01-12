using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionOutboxPatternApp.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error_message",
                table: "transaction_outbox",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_message",
                table: "transaction_outbox");
        }
    }
}
