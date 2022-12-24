using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Data.Migrations
{
    public partial class ThirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "CompanyAcronym",
                table: "Vehicle");

            migrationBuilder.RenameColumn(
                name: "offDate",
                table: "Reservation",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "inDate",
                table: "Reservation",
                newName: "EndDate");

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Vehicle",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Reservation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DamageEnd",
                table: "Reservation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DamageStart",
                table: "Reservation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "KmsEnd",
                table: "Reservation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "KmsStart",
                table: "Reservation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsEnd",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsStart",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Available = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CompanyId",
                table: "Vehicle",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Company_CompanyId",
                table: "Vehicle",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Company_CompanyId",
                table: "Vehicle");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_CompanyId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DamageEnd",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DamageStart",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "KmsEnd",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "KmsStart",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ObservationsEnd",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ObservationsStart",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Reservation",
                newName: "offDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Reservation",
                newName: "inDate");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAcronym",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
