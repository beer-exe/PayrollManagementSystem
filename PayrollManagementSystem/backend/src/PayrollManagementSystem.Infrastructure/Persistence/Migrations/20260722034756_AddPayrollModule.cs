using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ky_luongs",
                columns: table => new
                {
                    id_ky_luong = table.Column<Guid>(type: "uuid", nullable: false),
                    thang = table.Column<int>(type: "integer", nullable: false),
                    nam = table.Column<int>(type: "integer", nullable: false),
                    ten_ky_luong = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_ket_thuc = table.Column<DateOnly>(type: "date", nullable: false),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ky_luongs_pkey", x => x.id_ky_luong);
                });

            migrationBuilder.CreateTable(
                name: "bang_luongs",
                columns: table => new
                {
                    id_bang_luong = table.Column<Guid>(type: "uuid", nullable: false),
                    id_ky_luong = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    thang = table.Column<int>(type: "integer", nullable: false),
                    nam = table.Column<int>(type: "integer", nullable: false),
                    p1 = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    he_so_p2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    he_so_p3 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ngay_cong_chuan = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ngay_cong_thuc_te = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    luong_thoi_gian = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    luong_hieu_suat_p3 = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    phu_cap = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    thuong = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tang_ca = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    phat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tru_bao_hiem = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tru_thue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tong_thu_nhap = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    thuc_linh = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ghi_chu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bang_luongs_pkey", x => x.id_bang_luong);
                    table.ForeignKey(
                        name: "bang_luongs_cccd_nhan_vien_fkey",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "bang_luongs_id_ky_luong_fkey",
                        column: x => x.id_ky_luong,
                        principalTable: "ky_luongs",
                        principalColumn: "id_ky_luong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_bang_luong_ky_nv",
                table: "bang_luongs",
                columns: new[] { "id_ky_luong", "cccd_nhan_vien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bang_luongs_cccd_nhan_vien",
                table: "bang_luongs",
                column: "cccd_nhan_vien");

            migrationBuilder.CreateIndex(
                name: "idx_ky_luong_thang_nam",
                table: "ky_luongs",
                columns: new[] { "thang", "nam" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bang_luongs");

            migrationBuilder.DropTable(
                name: "ky_luongs");
        }
    }
}
