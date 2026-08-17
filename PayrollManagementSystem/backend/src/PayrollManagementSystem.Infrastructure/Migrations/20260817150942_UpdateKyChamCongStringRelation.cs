using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKyChamCongStringRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "trang_thai",
                table: "ky_cham_congs",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "id_ky_cham_cong",
                table: "cham_congs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_cham_congs_id_ky_cham_cong",
                table: "cham_congs",
                column: "id_ky_cham_cong");

            migrationBuilder.AddForeignKey(
                name: "fk_cham_congs_ky_cham_congs_id_ky_cham_cong",
                table: "cham_congs",
                column: "id_ky_cham_cong",
                principalTable: "ky_cham_congs",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cham_congs_ky_cham_congs_id_ky_cham_cong",
                table: "cham_congs");

            migrationBuilder.DropIndex(
                name: "ix_cham_congs_id_ky_cham_cong",
                table: "cham_congs");

            migrationBuilder.DropColumn(
                name: "id_ky_cham_cong",
                table: "cham_congs");

            migrationBuilder.AlterColumn<int>(
                name: "trang_thai",
                table: "ky_cham_congs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
