using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeManagementModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hop_dong_lao_dongs",
                columns: table => new
                {
                    so_hop_dong = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    loai_hop_dong = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_ket_thuc = table.Column<DateOnly>(type: "date", nullable: true),
                    luong_co_ban = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("hop_dong_lao_dongs_pkey", x => x.so_hop_dong);
                    table.ForeignKey(
                        name: "hop_dong_lao_dongs_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nhat_ky_trang_thais",
                columns: table => new
                {
                    id_nhat_ky = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    trang_thai_cu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    trang_thai_moi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ly_do = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ngay_thay_doi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    nguoi_thay_doi = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("nhat_ky_trang_thais_pkey", x => x.id_nhat_ky);
                    table.ForeignKey(
                        name: "nhat_ky_trang_thais_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tai_lieu_nhan_viens",
                columns: table => new
                {
                    id_tai_lieu = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ten_tai_lieu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    loai_tai_lieu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    duong_dan_file = table.Column<string>(type: "text", nullable: false),
                    ngay_tai_len = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_hop_dong_lao_dongs_cccd",
                table: "hop_dong_lao_dongs",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_nhat_ky_trang_thais_cccd",
                table: "nhat_ky_trang_thais",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_tai_lieu_nhan_viens_cccd",
                table: "tai_lieu_nhan_viens",
                column: "cccd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hop_dong_lao_dongs");

            migrationBuilder.DropTable(
                name: "nhat_ky_trang_thais");

            migrationBuilder.DropTable(
                name: "tai_lieu_nhan_viens");
        }
    }
}
