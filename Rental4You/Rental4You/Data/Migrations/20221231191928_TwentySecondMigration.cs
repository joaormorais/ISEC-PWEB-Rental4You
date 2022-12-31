using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Data.Migrations
{
    public partial class TwentySecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyApplicationUser_AspNetUsers_ApplicationUserId1",
                table: "CompanyApplicationUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyApplicationUser_Company_CompanyId",
                table: "CompanyApplicationUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyApplicationUser",
                table: "CompanyApplicationUser");

            migrationBuilder.RenameTable(
                name: "CompanyApplicationUser",
                newName: "CompanyApplicationUsers");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyApplicationUser_CompanyId",
                table: "CompanyApplicationUsers",
                newName: "IX_CompanyApplicationUsers_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyApplicationUser_ApplicationUserId1",
                table: "CompanyApplicationUsers",
                newName: "IX_CompanyApplicationUsers_ApplicationUserId1");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId1",
                table: "CompanyApplicationUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyApplicationUsers",
                table: "CompanyApplicationUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyApplicationUsers_AspNetUsers_ApplicationUserId1",
                table: "CompanyApplicationUsers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyApplicationUsers_Company_CompanyId",
                table: "CompanyApplicationUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyApplicationUsers_AspNetUsers_ApplicationUserId1",
                table: "CompanyApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyApplicationUsers_Company_CompanyId",
                table: "CompanyApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyApplicationUsers",
                table: "CompanyApplicationUsers");

            migrationBuilder.RenameTable(
                name: "CompanyApplicationUsers",
                newName: "CompanyApplicationUser");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyApplicationUsers_CompanyId",
                table: "CompanyApplicationUser",
                newName: "IX_CompanyApplicationUser_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyApplicationUsers_ApplicationUserId1",
                table: "CompanyApplicationUser",
                newName: "IX_CompanyApplicationUser_ApplicationUserId1");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId1",
                table: "CompanyApplicationUser",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyApplicationUser",
                table: "CompanyApplicationUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyApplicationUser_AspNetUsers_ApplicationUserId1",
                table: "CompanyApplicationUser",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyApplicationUser_Company_CompanyId",
                table: "CompanyApplicationUser",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
