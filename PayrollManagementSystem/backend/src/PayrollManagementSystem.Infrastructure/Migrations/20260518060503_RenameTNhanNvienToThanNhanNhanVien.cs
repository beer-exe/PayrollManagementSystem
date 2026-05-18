using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTNhanNvienToThanNhanNhanVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tnhan_nviens");

            migrationBuilder.CreateTable(
                name: "than_nhan_nhan_vien",
                columns: table => new
                {
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ma_dinh_danh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_mqh = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("than_nhan_nhan_vien_pkey", x => new { x.cccd, x.ma_dinh_danh });
                    table.ForeignKey(
                        name: "than_nhan_nhan_vien_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "than_nhan_nhan_vien_id_mqh_fkey",
                        column: x => x.id_mqh,
                        principalTable: "moi_quan_hes",
                        principalColumn: "id_mqh",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "than_nhan_nhan_vien_ma_dinh_danh_fkey",
                        column: x => x.ma_dinh_danh,
                        principalTable: "than_nhans",
                        principalColumn: "ma_dinh_danh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_than_nhan_nhan_vien_id_mqh",
                table: "than_nhan_nhan_vien",
                column: "id_mqh");

            migrationBuilder.CreateIndex(
                name: "IX_than_nhan_nhan_vien_ma_dinh_danh",
                table: "than_nhan_nhan_vien",
                column: "ma_dinh_danh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "than_nhan_nhan_vien");

            migrationBuilder.CreateTable(
                name: "tnhan_nviens",
                columns: table => new
                {
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ma_dinh_danh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_mqh = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tnhan_nviens_pkey", x => new { x.cccd, x.ma_dinh_danh });
                    table.ForeignKey(
                        name: "tnhan_nviens_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tnhan_nviens_id_mqh_fkey",
                        column: x => x.id_mqh,
                        principalTable: "moi_quan_hes",
                        principalColumn: "id_mqh",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "tnhan_nviens_ma_dinh_danh_fkey",
                        column: x => x.ma_dinh_danh,
                        principalTable: "than_nhans",
                        principalColumn: "ma_dinh_danh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tnhan_nviens_id_mqh",
                table: "tnhan_nviens",
                column: "id_mqh");

            migrationBuilder.CreateIndex(
                name: "IX_tnhan_nviens_ma_dinh_danh",
                table: "tnhan_nviens",
                column: "ma_dinh_danh");
        }
    }
}
