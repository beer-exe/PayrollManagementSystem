using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPositionModule3P : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrangThai",
                table: "chuc_vus",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "mo_ta_cong_viec",
                table: "chuc_vus",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThai",
                table: "bac_luongs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ngay_ket_thuc",
                table: "bac_luongs",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ten_bac_luong",
                table: "bac_luongs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "khung_hieu_suat_p3",
                columns: table => new
                {
                    id_kpi = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_chuc_vu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ten_kpi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ty_trong = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("khung_hieu_suat_p3_pkey", x => x.id_kpi);
                    table.ForeignKey(
                        name: "khung_hieu_suat_id_chuc_vu_fkey",
                        column: x => x.id_chuc_vu,
                        principalTable: "chuc_vus",
                        principalColumn: "id_chuc_vu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "khung_nang_luc_p2",
                columns: table => new
                {
                    id_tieu_chi = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_chuc_vu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ten_nang_luc = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    yeu_cau_toi_thieu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    diem_chuan = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("khung_nang_luc_p2_pkey", x => x.id_tieu_chi);
                    table.ForeignKey(
                        name: "khung_nang_luc_id_chuc_vu_fkey",
                        column: x => x.id_chuc_vu,
                        principalTable: "chuc_vus",
                        principalColumn: "id_chuc_vu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_khung_hieu_suat_p3_id_chuc_vu",
                table: "khung_hieu_suat_p3",
                column: "id_chuc_vu");

            migrationBuilder.CreateIndex(
                name: "IX_khung_nang_luc_p2_id_chuc_vu",
                table: "khung_nang_luc_p2",
                column: "id_chuc_vu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "khung_hieu_suat_p3");

            migrationBuilder.DropTable(
                name: "khung_nang_luc_p2");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "mo_ta_cong_viec",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "bac_luongs");

            migrationBuilder.DropColumn(
                name: "ngay_ket_thuc",
                table: "bac_luongs");

            migrationBuilder.DropColumn(
                name: "ten_bac_luong",
                table: "bac_luongs");
        }
    }
}
