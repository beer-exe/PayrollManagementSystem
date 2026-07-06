using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDiemChuanFromKhungNangLucP2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "diem_chuan",
                table: "khung_nang_luc_p2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "diem_chuan",
                table: "khung_nang_luc_p2",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
