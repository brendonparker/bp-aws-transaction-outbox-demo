using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BP.TransactionalOutboxDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "message_group_id",
                table: "transaction_outbox",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "message_group_id",
                table: "transaction_outbox");
        }
    }
}
