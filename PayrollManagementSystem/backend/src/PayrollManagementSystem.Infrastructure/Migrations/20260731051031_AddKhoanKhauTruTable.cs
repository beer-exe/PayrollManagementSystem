using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKhoanKhauTruTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "khoan_khau_trus",
                columns: table => new
                {
                    id_khoan_khau_tru = table.Column<Guid>(type: "uuid", nullable: false),
                    ten_khoan_khau_tru = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    loai_cong_thuc = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    gia_tri = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ghi_chu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    thu_tu_hien_thi = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("khoan_khau_trus_pkey", x => x.id_khoan_khau_tru);
                });

            migrationBuilder.CreateIndex(
                name: "idx_khoan_khau_tru_ten",
                table: "khoan_khau_trus",
                column: "ten_khoan_khau_tru",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "khoan_khau_trus");
        }
    }
}
