using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepremApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEventId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "Depremler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Depremler");
        }
    }
}
