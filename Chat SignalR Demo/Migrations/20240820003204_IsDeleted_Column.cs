using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat_SignalR_Demo.Migrations
{
    /// <inheritdoc />
    public partial class IsDeleted_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "conversation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "conversation");
        }
    }
}
