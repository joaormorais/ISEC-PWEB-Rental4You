using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Data.Migrations
{
    public partial class EighteenthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ended",
                table: "Reservation",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ended",
                table: "Reservation");
        }
    }
}
