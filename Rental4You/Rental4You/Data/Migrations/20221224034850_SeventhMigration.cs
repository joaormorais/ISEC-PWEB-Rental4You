using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Data.Migrations
{
    public partial class SeventhMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_AspNetUsers_ApplicationUserId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_ApplicationUserId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Reservation");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Reservation",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "DamageImages",
                table: "Reservation",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateTable(
                name: "ApplicationUserCompany",
                columns: table => new
                {
                    CompaniesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserCompany", x => new { x.CompaniesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserCompany_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserCompany_Company_CompaniesId",
                        column: x => x.CompaniesId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_ClientId",
                table: "Reservation",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserCompany_UsersId",
                table: "ApplicationUserCompany",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_AspNetUsers_ClientId",
                table: "Reservation",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_AspNetUsers_ClientId",
                table: "Reservation");

            migrationBuilder.DropTable(
                name: "ApplicationUserCompany");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_ClientId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DamageImages",
                table: "Reservation");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Reservation",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_ApplicationUserId",
                table: "Reservation",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_AspNetUsers_ApplicationUserId",
                table: "Reservation",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
