using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameKpiTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_kpi_phieu_kpi_id_phieu_kpi",
                table: "ChiTietKpi");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_kpi_ky_kpi_id_ky_kpi",
                table: "PhieuKpi");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_kpi_nhan_viens_cccd_nhan_vien",
                table: "PhieuKpi");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_kpi_nhan_viens_cccd_quan_ly",
                table: "PhieuKpi");

            migrationBuilder.DropPrimaryKey(
                name: "pk_phieu_kpi",
                table: "PhieuKpi");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ky_kpi",
                table: "KyKpi");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chi_tiet_kpi",
                table: "ChiTietKpi");

            migrationBuilder.RenameTable(
                name: "PhieuKpi",
                newName: "phieu_kpis");

            migrationBuilder.RenameTable(
                name: "KyKpi",
                newName: "ky_kpis");

            migrationBuilder.RenameTable(
                name: "ChiTietKpi",
                newName: "chi_tiet_kpis");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_kpi_id_ky_kpi_cccd_nhan_vien",
                table: "phieu_kpis",
                newName: "ix_phieu_kpis_id_ky_kpi_cccd_nhan_vien");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_kpi_cccd_quan_ly",
                table: "phieu_kpis",
                newName: "ix_phieu_kpis_cccd_quan_ly");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_kpi_cccd_nhan_vien",
                table: "phieu_kpis",
                newName: "ix_phieu_kpis_cccd_nhan_vien");

            migrationBuilder.RenameIndex(
                name: "ix_ky_kpi_thang_nam",
                table: "ky_kpis",
                newName: "ix_ky_kpis_thang_nam");

            migrationBuilder.RenameIndex(
                name: "ix_chi_tiet_kpi_id_phieu_kpi",
                table: "chi_tiet_kpis",
                newName: "ix_chi_tiet_kpis_id_phieu_kpi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_phieu_kpis",
                table: "phieu_kpis",
                column: "id_phieu_kpi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ky_kpis",
                table: "ky_kpis",
                column: "id_ky_kpi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chi_tiet_kpis",
                table: "chi_tiet_kpis",
                column: "id_chi_tiet_kpi");

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_kpis_phieu_kpis_id_phieu_kpi",
                table: "chi_tiet_kpis",
                column: "id_phieu_kpi",
                principalTable: "phieu_kpis",
                principalColumn: "id_phieu_kpi",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_kpis_ky_kpis_id_ky_kpi",
                table: "phieu_kpis",
                column: "id_ky_kpi",
                principalTable: "ky_kpis",
                principalColumn: "id_ky_kpi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_kpis_nhan_viens_cccd_nhan_vien",
                table: "phieu_kpis",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_kpis_nhan_viens_cccd_quan_ly",
                table: "phieu_kpis",
                column: "cccd_quan_ly",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_kpis_phieu_kpis_id_phieu_kpi",
                table: "chi_tiet_kpis");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_kpis_ky_kpis_id_ky_kpi",
                table: "phieu_kpis");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_kpis_nhan_viens_cccd_nhan_vien",
                table: "phieu_kpis");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_kpis_nhan_viens_cccd_quan_ly",
                table: "phieu_kpis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_phieu_kpis",
                table: "phieu_kpis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ky_kpis",
                table: "ky_kpis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chi_tiet_kpis",
                table: "chi_tiet_kpis");

            migrationBuilder.RenameTable(
                name: "phieu_kpis",
                newName: "PhieuKpi");

            migrationBuilder.RenameTable(
                name: "ky_kpis",
                newName: "KyKpi");

            migrationBuilder.RenameTable(
                name: "chi_tiet_kpis",
                newName: "ChiTietKpi");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_kpis_id_ky_kpi_cccd_nhan_vien",
                table: "PhieuKpi",
                newName: "ix_phieu_kpi_id_ky_kpi_cccd_nhan_vien");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_kpis_cccd_quan_ly",
                table: "PhieuKpi",
                newName: "ix_phieu_kpi_cccd_quan_ly");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_kpis_cccd_nhan_vien",
                table: "PhieuKpi",
                newName: "ix_phieu_kpi_cccd_nhan_vien");

            migrationBuilder.RenameIndex(
                name: "ix_ky_kpis_thang_nam",
                table: "KyKpi",
                newName: "ix_ky_kpi_thang_nam");

            migrationBuilder.RenameIndex(
                name: "ix_chi_tiet_kpis_id_phieu_kpi",
                table: "ChiTietKpi",
                newName: "ix_chi_tiet_kpi_id_phieu_kpi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_phieu_kpi",
                table: "PhieuKpi",
                column: "id_phieu_kpi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ky_kpi",
                table: "KyKpi",
                column: "id_ky_kpi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chi_tiet_kpi",
                table: "ChiTietKpi",
                column: "id_chi_tiet_kpi");

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_kpi_phieu_kpi_id_phieu_kpi",
                table: "ChiTietKpi",
                column: "id_phieu_kpi",
                principalTable: "PhieuKpi",
                principalColumn: "id_phieu_kpi",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_kpi_ky_kpi_id_ky_kpi",
                table: "PhieuKpi",
                column: "id_ky_kpi",
                principalTable: "KyKpi",
                principalColumn: "id_ky_kpi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_kpi_nhan_viens_cccd_nhan_vien",
                table: "PhieuKpi",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_kpi_nhan_viens_cccd_quan_ly",
                table: "PhieuKpi",
                column: "cccd_quan_ly",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
