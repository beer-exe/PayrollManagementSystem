using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLichLamViecUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs");

            migrationBuilder.CreateIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs",
                column: "nam",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs");

            migrationBuilder.CreateIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs",
                column: "nam",
                unique: true);
        }
    }
}
