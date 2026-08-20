using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBacThueAndCauHinhGiamTru : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bac_thues",
                columns: table => new
                {
                    id_bac_thue = table.Column<Guid>(type: "uuid", nullable: false),
                    bac = table.Column<int>(type: "integer", nullable: false),
                    tu_gia = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    den_gia = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    thue_suat = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bac_thues_pkey", x => x.id_bac_thue);
                });

            migrationBuilder.CreateTable(
                name: "cau_hinh_giam_trus",
                columns: table => new
                {
                    id_cau_hinh_giam_tru = table.Column<Guid>(type: "uuid", nullable: false),
                    giam_tru_ban_than = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    giam_tru_nguoi_phu_thuoc = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ghi_chu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cau_hinh_giam_trus_pkey", x => x.id_cau_hinh_giam_tru);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bac_thues");

            migrationBuilder.DropTable(
                name: "cau_hinh_giam_trus");
        }
    }
}
