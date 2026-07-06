using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleDanhGiaNangLucP2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cccd_nguoi_quan_ly",
                table: "nhan_viens",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ty_trong",
                table: "khung_nang_luc_p2",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ky_danh_gias",
                columns: table => new
                {
                    id_ky_danh_gia = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ten_ky_danh_gia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nam = table.Column<int>(type: "integer", nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_ket_thuc = table.Column<DateOnly>(type: "date", nullable: false),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ky_danh_gias_pkey", x => x.id_ky_danh_gia);
                });

            migrationBuilder.CreateTable(
                name: "muc_quy_doi_p2s",
                columns: table => new
                {
                    id_quy_doi = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    xep_loai = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    diem_toi_thieu = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    diem_toi_da = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    he_so_p2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("muc_quy_doi_p2s_pkey", x => x.id_quy_doi);
                });

            migrationBuilder.CreateTable(
                name: "phieu_danh_gia_nang_lucs",
                columns: table => new
                {
                    id_phieu = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_ky_danh_gia = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cccd_quan_ly = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    diem_tong_hop = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    he_so_p2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    xep_loai = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    nhan_xet_chung = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phieu_danh_gia_nang_lucs_pkey", x => x.id_phieu);
                    table.ForeignKey(
                        name: "phieu_danh_gias_cccd_nhan_vien_fkey",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "phieu_danh_gias_cccd_quan_ly_fkey",
                        column: x => x.cccd_quan_ly,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "phieu_danh_gias_id_ky_danh_gia_fkey",
                        column: x => x.id_ky_danh_gia,
                        principalTable: "ky_danh_gias",
                        principalColumn: "id_ky_danh_gia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chi_tiet_danh_gia_nang_lucs",
                columns: table => new
                {
                    id_chi_tiet = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_phieu = table.Column<Guid>(type: "uuid", nullable: false),
                    id_tieu_chi = table.Column<Guid>(type: "uuid", nullable: false),
                    diem_tu_danh_gia = table.Column<int>(type: "integer", nullable: true),
                    diem_quan_ly_danh_gia = table.Column<int>(type: "integer", nullable: true),
                    nhan_xet_nhan_vien = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    nhan_xet_quan_ly = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chi_tiet_danh_gia_nang_lucs_pkey", x => x.id_chi_tiet);
                    table.ForeignKey(
                        name: "chi_tiet_danh_gias_id_phieu_fkey",
                        column: x => x.id_phieu,
                        principalTable: "phieu_danh_gia_nang_lucs",
                        principalColumn: "id_phieu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "chi_tiet_danh_gias_id_tieu_chi_fkey",
                        column: x => x.id_tieu_chi,
                        principalTable: "khung_nang_luc_p2",
                        principalColumn: "id_tieu_chi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "he_so_nang_luc_nhan_viens",
                columns: table => new
                {
                    id_he_so = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    he_so_p2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    id_phieu_danh_gia = table.Column<Guid>(type: "uuid", nullable: true),
                    ngay_hieu_luc = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_het_han = table.Column<DateOnly>(type: "date", nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("he_so_nang_luc_nhan_viens_pkey", x => x.id_he_so);
                    table.ForeignKey(
                        name: "he_so_nang_lucs_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "he_so_nang_lucs_id_phieu_danh_gia_fkey",
                        column: x => x.id_phieu_danh_gia,
                        principalTable: "phieu_danh_gia_nang_lucs",
                        principalColumn: "id_phieu",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nhan_viens_cccd_nguoi_quan_ly",
                table: "nhan_viens",
                column: "cccd_nguoi_quan_ly");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_danh_gia_nang_lucs_id_phieu",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_phieu");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_danh_gia_nang_lucs_id_tieu_chi",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_tieu_chi");

            migrationBuilder.CreateIndex(
                name: "IX_he_so_nang_luc_nhan_viens_cccd",
                table: "he_so_nang_luc_nhan_viens",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_he_so_nang_luc_nhan_viens_id_phieu_danh_gia",
                table: "he_so_nang_luc_nhan_viens",
                column: "id_phieu_danh_gia");

            migrationBuilder.CreateIndex(
                name: "IX_phieu_danh_gia_nang_lucs_cccd_nhan_vien",
                table: "phieu_danh_gia_nang_lucs",
                column: "cccd_nhan_vien");

            migrationBuilder.CreateIndex(
                name: "IX_phieu_danh_gia_nang_lucs_cccd_quan_ly",
                table: "phieu_danh_gia_nang_lucs",
                column: "cccd_quan_ly");

            migrationBuilder.CreateIndex(
                name: "IX_phieu_danh_gia_nang_lucs_id_ky_danh_gia",
                table: "phieu_danh_gia_nang_lucs",
                column: "id_ky_danh_gia");

            migrationBuilder.AddForeignKey(
                name: "nhan_viens_cccd_nguoi_quan_ly_fkey",
                table: "nhan_viens",
                column: "cccd_nguoi_quan_ly",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "nhan_viens_cccd_nguoi_quan_ly_fkey",
                table: "nhan_viens");

            migrationBuilder.DropTable(
                name: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropTable(
                name: "he_so_nang_luc_nhan_viens");

            migrationBuilder.DropTable(
                name: "muc_quy_doi_p2s");

            migrationBuilder.DropTable(
                name: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropTable(
                name: "ky_danh_gias");

            migrationBuilder.DropIndex(
                name: "IX_nhan_viens_cccd_nguoi_quan_ly",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "cccd_nguoi_quan_ly",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "ty_trong",
                table: "khung_nang_luc_p2");
        }
    }
}
