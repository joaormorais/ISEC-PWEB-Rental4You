using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Data.Migrations
{
    public partial class TwentyFourthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyApplicationUsers_AspNetUsers_ApplicationUserId1",
                table: "CompanyApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyApplicationUsers_ApplicationUserId1",
                table: "CompanyApplicationUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "CompanyApplicationUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "CompanyApplicationUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NewUserId",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyApplicationUsers_ApplicationUserId",
                table: "CompanyApplicationUsers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyApplicationUsers_AspNetUsers_ApplicationUserId",
                table: "CompanyApplicationUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyApplicationUsers_AspNetUsers_ApplicationUserId",
                table: "CompanyApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyApplicationUsers_ApplicationUserId",
                table: "CompanyApplicationUsers");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "CompanyApplicationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "CompanyApplicationUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewUserId",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyApplicationUsers_ApplicationUserId1",
                table: "CompanyApplicationUsers",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyApplicationUsers_AspNetUsers_ApplicationUserId1",
                table: "CompanyApplicationUsers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
