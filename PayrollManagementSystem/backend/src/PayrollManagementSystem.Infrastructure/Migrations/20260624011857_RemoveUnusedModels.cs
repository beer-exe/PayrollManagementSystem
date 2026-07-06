using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "he_so_nang_luc_nhan_viens");

            migrationBuilder.DropTable(
                name: "khung_hieu_suat_p3");

            migrationBuilder.DropTable(
                name: "tai_khoan_ngan_hangs");

            migrationBuilder.DropTable(
                name: "tai_lieu_nhan_viens");

            migrationBuilder.DropTable(
                name: "ngan_hangs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "he_so_nang_luc_nhan_viens",
                columns: table => new
                {
                    id_he_so = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    id_phieu_danh_gia = table.Column<Guid>(type: "uuid", nullable: true),
                    he_so_p2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ngay_het_han = table.Column<DateOnly>(type: "date", nullable: true),
                    ngay_hieu_luc = table.Column<DateOnly>(type: "date", nullable: false),
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
                name: "ngan_hangs",
                columns: table => new
                {
                    id_ngan_hang = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ten_ngan_hang = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ngan_hangs_pkey", x => x.id_ngan_hang);
                });

            migrationBuilder.CreateTable(
                name: "tai_lieu_nhan_viens",
                columns: table => new
                {
                    id_tai_lieu = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    duong_dan_file = table.Column<string>(type: "text", nullable: false),
                    loai_tai_lieu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ngay_tai_len = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ten_tai_lieu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tai_lieu_nhan_viens_pkey", x => x.id_tai_lieu);
                    table.ForeignKey(
                        name: "tai_lieu_nhan_viens_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tai_khoan_ngan_hangs",
                columns: table => new
                {
                    stk = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    id_ngan_hang = table.Column<Guid>(type: "uuid", nullable: true),
                    chi_nhanh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ngay_mo_the = table.Column<DateOnly>(type: "date", nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tai_khoan_ngan_hangs_pkey", x => x.stk);
                    table.ForeignKey(
                        name: "tai_khoan_ngan_hangs_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tai_khoan_ngan_hangs_id_ngan_hang_fkey",
                        column: x => x.id_ngan_hang,
                        principalTable: "ngan_hangs",
                        principalColumn: "id_ngan_hang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_he_so_nang_luc_nhan_viens_cccd",
                table: "he_so_nang_luc_nhan_viens",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_he_so_nang_luc_nhan_viens_id_phieu_danh_gia",
                table: "he_so_nang_luc_nhan_viens",
                column: "id_phieu_danh_gia");

            migrationBuilder.CreateIndex(
                name: "IX_khung_hieu_suat_p3_id_chuc_vu",
                table: "khung_hieu_suat_p3",
                column: "id_chuc_vu");

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_ngan_hangs_cccd",
                table: "tai_khoan_ngan_hangs",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_ngan_hangs_id_ngan_hang",
                table: "tai_khoan_ngan_hangs",
                column: "id_ngan_hang");

            migrationBuilder.CreateIndex(
                name: "IX_tai_lieu_nhan_viens_cccd",
                table: "tai_lieu_nhan_viens",
                column: "cccd");
        }
    }
}
