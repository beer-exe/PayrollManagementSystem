using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNgachLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM bac_luongs;");

            migrationBuilder.DropForeignKey(
                name: "bac_luongs_id_chuc_vu_fkey",
                table: "bac_luongs");

            migrationBuilder.RenameColumn(
                name: "id_chuc_vu",
                table: "bac_luongs",
                newName: "id_ngach_luong");

            migrationBuilder.RenameIndex(
                name: "IX_bac_luongs_id_chuc_vu",
                table: "bac_luongs",
                newName: "IX_bac_luongs_id_ngach_luong");

            migrationBuilder.AddColumn<string>(
                name: "id_ngach_luong",
                table: "chuc_vus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ngach_luongs",
                columns: table => new
                {
                    id_ngach_luong = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ten_ngach_luong = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mo_ta = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ngach_luongs_pkey", x => x.id_ngach_luong);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chuc_vus_id_ngach_luong",
                table: "chuc_vus",
                column: "id_ngach_luong");

            migrationBuilder.AddForeignKey(
                name: "bac_luongs_id_ngach_luong_fkey",
                table: "bac_luongs",
                column: "id_ngach_luong",
                principalTable: "ngach_luongs",
                principalColumn: "id_ngach_luong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "chuc_vus_id_ngach_luong_fkey",
                table: "chuc_vus",
                column: "id_ngach_luong",
                principalTable: "ngach_luongs",
                principalColumn: "id_ngach_luong",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "bac_luongs_id_ngach_luong_fkey",
                table: "bac_luongs");

            migrationBuilder.DropForeignKey(
                name: "chuc_vus_id_ngach_luong_fkey",
                table: "chuc_vus");

            migrationBuilder.DropTable(
                name: "ngach_luongs");

            migrationBuilder.DropIndex(
                name: "IX_chuc_vus_id_ngach_luong",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "id_ngach_luong",
                table: "chuc_vus");

            migrationBuilder.RenameColumn(
                name: "id_ngach_luong",
                table: "bac_luongs",
                newName: "id_chuc_vu");

            migrationBuilder.RenameIndex(
                name: "IX_bac_luongs_id_ngach_luong",
                table: "bac_luongs",
                newName: "IX_bac_luongs_id_chuc_vu");

            migrationBuilder.AddForeignKey(
                name: "bac_luongs_id_chuc_vu_fkey",
                table: "bac_luongs",
                column: "id_chuc_vu",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
