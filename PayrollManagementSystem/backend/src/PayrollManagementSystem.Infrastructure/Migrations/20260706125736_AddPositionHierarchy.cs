using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa dữ liệu cũ vì thay đổi cấu trúc thiết yếu và yêu cầu IdPhongBan non-null
            migrationBuilder.Sql("DELETE FROM quyet_dinh_nhan_sus;");
            migrationBuilder.Sql("DELETE FROM chuc_vus;");

            migrationBuilder.DropForeignKey(
                name: "nhan_viens_cccd_nguoi_quan_ly_fkey",
                table: "nhan_viens");

            migrationBuilder.DropIndex(
                name: "IX_nhan_viens_cccd_nguoi_quan_ly",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "cccd_nguoi_quan_ly",
                table: "nhan_viens");

            migrationBuilder.RenameColumn(
                name: "id_tai_khoan",
                table: "nhan_viens",
                newName: "IdTaiKhoan");

            migrationBuilder.RenameIndex(
                name: "IX_nhan_viens_id_tai_khoan",
                table: "nhan_viens",
                newName: "IX_nhan_viens_IdTaiKhoan");

            migrationBuilder.AddColumn<string>(
                name: "id_chuc_vu_quan_ly",
                table: "chuc_vus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id_phong_ban",
                table: "chuc_vus",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_chuc_vus_id_chuc_vu_quan_ly",
                table: "chuc_vus",
                column: "id_chuc_vu_quan_ly");

            migrationBuilder.CreateIndex(
                name: "IX_chuc_vus_id_phong_ban",
                table: "chuc_vus",
                column: "id_phong_ban");

            migrationBuilder.AddForeignKey(
                name: "chuc_vus_id_chuc_vu_quan_ly_fkey",
                table: "chuc_vus",
                column: "id_chuc_vu_quan_ly",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "chuc_vus_id_phong_ban_fkey",
                table: "chuc_vus",
                column: "id_phong_ban",
                principalTable: "phong_bans",
                principalColumn: "id_pb",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "chuc_vus_id_chuc_vu_quan_ly_fkey",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "chuc_vus_id_phong_ban_fkey",
                table: "chuc_vus");

            migrationBuilder.DropIndex(
                name: "IX_chuc_vus_id_chuc_vu_quan_ly",
                table: "chuc_vus");

            migrationBuilder.DropIndex(
                name: "IX_chuc_vus_id_phong_ban",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "id_chuc_vu_quan_ly",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "id_phong_ban",
                table: "chuc_vus");

            migrationBuilder.RenameColumn(
                name: "IdTaiKhoan",
                table: "nhan_viens",
                newName: "id_tai_khoan");

            migrationBuilder.RenameIndex(
                name: "IX_nhan_viens_IdTaiKhoan",
                table: "nhan_viens",
                newName: "IX_nhan_viens_id_tai_khoan");

            migrationBuilder.AddColumn<string>(
                name: "cccd_nguoi_quan_ly",
                table: "nhan_viens",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_nhan_viens_cccd_nguoi_quan_ly",
                table: "nhan_viens",
                column: "cccd_nguoi_quan_ly");

            migrationBuilder.AddForeignKey(
                name: "nhan_viens_cccd_nguoi_quan_ly_fkey",
                table: "nhan_viens",
                column: "cccd_nguoi_quan_ly",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
