using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLichLamViec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lich_lam_viecs",
                columns: table => new
                {
                    id_lich = table.Column<Guid>(type: "uuid", nullable: false),
                    nam = table.Column<int>(type: "integer", nullable: false),
                    trang_thai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ghi_chu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lich_lam_viecs_pkey", x => x.id_lich);
                });

            migrationBuilder.CreateTable(
                name: "chi_tiet_lich_lam_viecs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_lich = table.Column<Guid>(type: "uuid", nullable: false),
                    ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    thu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    loai_ngay = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ten_ngay_nghi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    so_gio_lam = table.Column<decimal>(type: "numeric(4,1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chi_tiet_lich_lam_viecs_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_chi_tiet_lich_lam_viec",
                        column: x => x.id_lich,
                        principalTable: "lich_lam_viecs",
                        principalColumn: "id_lich",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_chi_tiet_lich_ngay",
                table: "chi_tiet_lich_lam_viecs",
                columns: new[] { "id_lich", "ngay" });

            migrationBuilder.CreateIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs",
                column: "nam",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropTable(
                name: "lich_lam_viecs");
        }
    }
}
