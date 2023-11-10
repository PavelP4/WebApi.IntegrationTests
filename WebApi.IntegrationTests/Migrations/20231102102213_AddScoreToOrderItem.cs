using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.IntegrationTests.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Score",
                table: "OrderItems",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "OrderItems");
        }
    }
}
