using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToTaiKhoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "tai_khoans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry_time",
                table: "tai_khoans",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "tai_khoans");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry_time",
                table: "tai_khoans");
        }
    }
}
