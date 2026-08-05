using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLaNguoiPhuThuocToTNhan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LaNguoiPhuThuoc",
                table: "than_nhan_nhan_vien",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaNguoiPhuThuoc",
                table: "than_nhan_nhan_vien");
        }
    }
}
