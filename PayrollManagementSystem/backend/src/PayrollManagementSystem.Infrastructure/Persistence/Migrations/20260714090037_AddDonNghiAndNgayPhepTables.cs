using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDonNghiAndNgayPhepTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "don_nghis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    loai_nghi = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_ket_thuc = table.Column<DateOnly>(type: "date", nullable: false),
                    so_ngay_nghi = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false),
                    ly_do = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tai_lieu_dinh_kem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cccd_nguoi_duyet = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ly_do_tu_choi = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ngay_duyet = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("don_nghis_pkey", x => x.id);
                    table.ForeignKey(
                        name: "don_nghis_cccd_nguoi_duyet_fkey",
                        column: x => x.cccd_nguoi_duyet,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "don_nghis_cccd_nhan_vien_fkey",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ngay_phep_nhan_viens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cccd_nhan_vien = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nam = table.Column<int>(type: "integer", nullable: false),
                    tong_ngay_phep = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false, defaultValue: 12m),
                    da_su_dung = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ngay_phep_nhan_viens_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ngay_phep_nhan_viens_cccd_fkey",
                        column: x => x.cccd_nhan_vien,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_don_nghis_nv_ngay",
                table: "don_nghis",
                columns: new[] { "cccd_nhan_vien", "ngay_bat_dau", "ngay_ket_thuc" });

            migrationBuilder.CreateIndex(
                name: "IX_don_nghis_cccd_nguoi_duyet",
                table: "don_nghis",
                column: "cccd_nguoi_duyet");

            migrationBuilder.CreateIndex(
                name: "ngay_phep_nhan_viens_cccd_nam_unique",
                table: "ngay_phep_nhan_viens",
                columns: new[] { "cccd_nhan_vien", "nam" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "don_nghis");

            migrationBuilder.DropTable(
                name: "ngay_phep_nhan_viens");
        }
    }
}
