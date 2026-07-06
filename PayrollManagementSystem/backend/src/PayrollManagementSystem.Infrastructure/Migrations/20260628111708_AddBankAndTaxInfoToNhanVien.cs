using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankAndTaxInfoToNhanVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ma_so_thue",
                table: "nhan_viens",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "so_tai_khoan",
                table: "nhan_viens",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ten_ngan_hang",
                table: "nhan_viens",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ma_so_thue",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "so_tai_khoan",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "ten_ngan_hang",
                table: "nhan_viens");
        }
    }
}
