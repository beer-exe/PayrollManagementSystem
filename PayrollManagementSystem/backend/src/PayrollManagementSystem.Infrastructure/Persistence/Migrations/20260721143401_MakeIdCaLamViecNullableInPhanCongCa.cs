using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeIdCaLamViecNullableInPhanCongCa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_phan_cong_ca_ca_lam_viec",
                table: "phan_cong_cas");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_ca_lam_viec",
                table: "phan_cong_cas",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_phan_cong_ca_ca_lam_viec",
                table: "phan_cong_cas",
                column: "id_ca_lam_viec",
                principalTable: "ca_lam_viecs",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_phan_cong_ca_ca_lam_viec",
                table: "phan_cong_cas");

            migrationBuilder.AlterColumn<Guid>(
                name: "id_ca_lam_viec",
                table: "phan_cong_cas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_phan_cong_ca_ca_lam_viec",
                table: "phan_cong_cas",
                column: "id_ca_lam_viec",
                principalTable: "ca_lam_viecs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
