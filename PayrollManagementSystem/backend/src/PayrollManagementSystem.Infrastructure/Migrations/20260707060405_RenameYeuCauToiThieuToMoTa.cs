using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameYeuCauToiThieuToMoTa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "yeu_cau_toi_thieu",
                table: "khung_nang_luc_p2",
                newName: "mo_ta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "mo_ta",
                table: "khung_nang_luc_p2",
                newName: "yeu_cau_toi_thieu");
        }
    }
}
