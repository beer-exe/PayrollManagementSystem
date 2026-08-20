using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChamCongTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cham_congs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ngay_cham_cong = table.Column<DateOnly>(type: "date", nullable: false),
                    gio_vao = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    gio_ra = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    so_gio_lam_thuc_te = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    so_ngay_cong = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    loai_ngay_cong = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_nhap_tay = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ghi_chu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cham_congs_pkey", x => x.id);
                    table.ForeignKey(
                        name: "cham_congs_cccd_nhan_vien_fkey",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "cham_congs_cccd_ngay_unique",
                table: "cham_congs",
                columns: new[] { "cccd_nhan_vien", "ngay_cham_cong" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cham_congs");
        }
    }
}
