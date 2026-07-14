using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuyetDinhNhanSuModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_quyet_dinh_nhan_sus_cccd",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.AddColumn<string>(
                name: "id_bac_luong_cu",
                table: "quyet_dinh_nhan_sus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id_chuc_vu_cu",
                table: "quyet_dinh_nhan_sus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_qd_nhansu_active",
                table: "quyet_dinh_nhan_sus",
                columns: new[] { "cccd", "trang_thai", "ngay_hieu_luc" });

            migrationBuilder.CreateIndex(
                name: "IX_quyet_dinh_nhan_sus_id_chuc_vu_moi",
                table: "quyet_dinh_nhan_sus",
                column: "id_chuc_vu_moi");

            migrationBuilder.Sql(@"
                UPDATE quyet_dinh_nhan_sus 
                SET id_chuc_vu_moi = NULL 
                WHERE id_chuc_vu_moi NOT IN (SELECT id_chuc_vu FROM chuc_vus);
            ");

            migrationBuilder.AddForeignKey(
                name: "quyet_dinh_nhan_sus_id_chuc_vu_moi_fkey",
                table: "quyet_dinh_nhan_sus",
                column: "id_chuc_vu_moi",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "quyet_dinh_nhan_sus_id_chuc_vu_moi_fkey",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropIndex(
                name: "idx_qd_nhansu_active",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropIndex(
                name: "IX_quyet_dinh_nhan_sus_id_chuc_vu_moi",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "id_bac_luong_cu",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "id_chuc_vu_cu",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.CreateIndex(
                name: "IX_quyet_dinh_nhan_sus_cccd",
                table: "quyet_dinh_nhan_sus",
                column: "cccd");
        }
    }
}
