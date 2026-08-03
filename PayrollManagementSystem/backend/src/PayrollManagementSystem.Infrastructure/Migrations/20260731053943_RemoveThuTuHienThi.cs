using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveThuTuHienThi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thu_tu_hien_thi",
                table: "khoan_khau_trus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "thu_tu_hien_thi",
                table: "khoan_khau_trus",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
