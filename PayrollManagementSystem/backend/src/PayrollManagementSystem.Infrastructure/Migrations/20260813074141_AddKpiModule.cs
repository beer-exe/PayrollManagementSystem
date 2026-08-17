using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKpiModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KyKpi",
                columns: table => new
                {
                    id_ky_kpi = table.Column<Guid>(type: "uuid", nullable: false),
                    ten_ky_kpi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    thang = table.Column<int>(type: "integer", nullable: false),
                    nam = table.Column<int>(type: "integer", nullable: false),
                    trang_thai = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ky_kpi", x => x.id_ky_kpi);
                });

            migrationBuilder.CreateTable(
                name: "PhieuKpi",
                columns: table => new
                {
                    id_phieu_kpi = table.Column<Guid>(type: "uuid", nullable: false),
                    id_ky_kpi = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    cccd_quan_ly = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    tong_diem_kpi = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    he_so_p3 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    nhan_xet = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    trang_thai = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_phieu_kpi", x => x.id_phieu_kpi);
                    table.ForeignKey(
                        name: "fk_phieu_kpi_ky_kpi_id_ky_kpi",
                        column: x => x.id_ky_kpi,
                        principalTable: "KyKpi",
                        principalColumn: "id_ky_kpi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_phieu_kpi_nhan_viens_cccd_nhan_vien",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_phieu_kpi_nhan_viens_cccd_quan_ly",
                        column: x => x.cccd_quan_ly,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKpi",
                columns: table => new
                {
                    id_chi_tiet_kpi = table.Column<Guid>(type: "uuid", nullable: false),
                    id_phieu_kpi = table.Column<Guid>(type: "uuid", nullable: false),
                    muc_tieu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    don_vi_tinh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    trong_so = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    chi_tieu = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    thuc_te = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ti_le_hoan_thanh = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    diem_kpi = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chi_tiet_kpi", x => x.id_chi_tiet_kpi);
                    table.ForeignKey(
                        name: "fk_chi_tiet_kpi_phieu_kpi_id_phieu_kpi",
                        column: x => x.id_phieu_kpi,
                        principalTable: "PhieuKpi",
                        principalColumn: "id_phieu_kpi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chi_tiet_kpi_id_phieu_kpi",
                table: "ChiTietKpi",
                column: "id_phieu_kpi");

            migrationBuilder.CreateIndex(
                name: "ix_ky_kpi_thang_nam",
                table: "KyKpi",
                columns: new[] { "thang", "nam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_phieu_kpi_cccd_nhan_vien",
                table: "PhieuKpi",
                column: "cccd_nhan_vien");

            migrationBuilder.CreateIndex(
                name: "ix_phieu_kpi_cccd_quan_ly",
                table: "PhieuKpi",
                column: "cccd_quan_ly");

            migrationBuilder.CreateIndex(
                name: "ix_phieu_kpi_id_ky_kpi_cccd_nhan_vien",
                table: "PhieuKpi",
                columns: new[] { "id_ky_kpi", "cccd_nhan_vien" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietKpi");

            migrationBuilder.DropTable(
                name: "PhieuKpi");

            migrationBuilder.DropTable(
                name: "KyKpi");
        }
    }
}
