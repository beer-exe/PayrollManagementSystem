using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkCaLamViecToLichLamViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ca_lam_viecs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ten_ca = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    gio_bat_dau = table.Column<TimeSpan>(type: "interval", nullable: false),
                    gio_ket_thuc = table.Column<TimeSpan>(type: "interval", nullable: false),
                    xuyen_ngay = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    he_so_luong = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 1.0m),
                    trang_thai = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ca_lam_viecs_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "khung_gio_nghis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_ca_lam_viec = table.Column<Guid>(type: "uuid", nullable: true),
                    ten_khoang_nghi = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    gio_bat_dau = table.Column<TimeSpan>(type: "interval", nullable: false),
                    gio_ket_thuc = table.Column<TimeSpan>(type: "interval", nullable: false),
                    tinh_vao_gio_lam = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("khung_gio_nghis_pkey", x => x.id);
                    table.ForeignKey(
                        name: "khung_gio_nghis_id_ca_lam_viec_fkey",
                        column: x => x.id_ca_lam_viec,
                        principalTable: "ca_lam_viecs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phan_cong_cas",
                columns: table => new
                {
                    id_phan_cong = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ngay_lam_viec = table.Column<DateOnly>(type: "date", nullable: false),
                    id_ca_lam_viec = table.Column<Guid>(type: "uuid", nullable: false),
                    ghi_chu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phan_cong_cas_pkey", x => x.id_phan_cong);
                    table.ForeignKey(
                        name: "fk_phan_cong_ca_ca_lam_viec",
                        column: x => x.id_ca_lam_viec,
                        principalTable: "ca_lam_viecs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_phan_cong_ca_nhan_vien",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_lich_lam_viecs_id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                column: "id_ca_lam_viec_mac_dinh");

            migrationBuilder.CreateIndex(
                name: "IX_khung_gio_nghis_id_ca_lam_viec",
                table: "khung_gio_nghis",
                column: "id_ca_lam_viec");

            migrationBuilder.CreateIndex(
                name: "idx_phan_cong_ca_nv_ngay_unique",
                table: "phan_cong_cas",
                columns: new[] { "cccd_nhan_vien", "ngay_lam_viec" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_phan_cong_cas_id_ca_lam_viec",
                table: "phan_cong_cas",
                column: "id_ca_lam_viec");

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_lich_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                column: "id_ca_lam_viec_mac_dinh",
                principalTable: "ca_lam_viecs",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_lich_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropTable(
                name: "khung_gio_nghis");

            migrationBuilder.DropTable(
                name: "phan_cong_cas");

            migrationBuilder.DropTable(
                name: "ca_lam_viecs");

            migrationBuilder.DropIndex(
                name: "IX_chi_tiet_lich_lam_viecs_id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropColumn(
                name: "id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs");
        }
    }
}
