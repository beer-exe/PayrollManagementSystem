using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkShiftTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_khung_gio_nghis_id_ca_lam_viec",
                table: "khung_gio_nghis",
                column: "id_ca_lam_viec");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "khung_gio_nghis");

            migrationBuilder.DropTable(
                name: "ca_lam_viecs");
        }
    }
}
