using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "chuc_vus",
                columns: table => new
                {
                    id_chuc_vu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ten_chuc_vu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chuc_vus_pkey", x => x.id_chuc_vu);
                });

            migrationBuilder.CreateTable(
                name: "moi_quan_hes",
                columns: table => new
                {
                    id_mqh = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ten_quan_he = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("moi_quan_hes_pkey", x => x.id_mqh);
                });

            migrationBuilder.CreateTable(
                name: "ngan_hangs",
                columns: table => new
                {
                    id_ngan_hang = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ten_ngan_hang = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ngan_hangs_pkey", x => x.id_ngan_hang);
                });

            migrationBuilder.CreateTable(
                name: "phong_bans",
                columns: table => new
                {
                    id_pb = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ten_pb = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phong_bans_pkey", x => x.id_pb);
                });

            migrationBuilder.CreateTable(
                name: "than_nhans",
                columns: table => new
                {
                    ma_dinh_danh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ten_tn = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ngay_sinh = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("than_nhans_pkey", x => x.ma_dinh_danh);
                });

            migrationBuilder.CreateTable(
                name: "vai_tros",
                columns: table => new
                {
                    id_vai_tro = table.Column<Guid>(type: "uuid", nullable: false),
                    ten_vai_tro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vai_tros_pkey", x => x.id_vai_tro);
                });

            migrationBuilder.CreateTable(
                name: "bac_luongs",
                columns: table => new
                {
                    id_bac_luong = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_chuc_vu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    luong_p1 = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ngay_ap_dung = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bac_luongs_pkey", x => x.id_bac_luong);
                    table.ForeignKey(
                        name: "bac_luongs_id_chuc_vu_fkey",
                        column: x => x.id_chuc_vu,
                        principalTable: "chuc_vus",
                        principalColumn: "id_chuc_vu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tai_khoans",
                columns: table => new
                {
                    id_tai_khoan = table.Column<Guid>(type: "uuid", nullable: false),
                    ten_tai_khoan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    mat_khau_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dang_nhap_lan_dau = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    id_vai_tro = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tai_khoans_pkey", x => x.id_tai_khoan);
                    table.ForeignKey(
                        name: "tai_khoans_id_vai_tro_fkey",
                        column: x => x.id_vai_tro,
                        principalTable: "vai_tros",
                        principalColumn: "id_vai_tro",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "nhan_viens",
                columns: table => new
                {
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ho_ten = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    gioi_tinh = table.Column<bool>(type: "boolean", nullable: true),
                    sdt = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ngay_sinh = table.Column<DateOnly>(type: "date", nullable: true),
                    dan_toc = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dia_chi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    chuyen_nganh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ngay_vao_lam = table.Column<DateOnly>(type: "date", nullable: true),
                    ngay_nghi_viec = table.Column<DateOnly>(type: "date", nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    so_bhxh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    so_bhyt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    id_pb = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    id_tai_khoan = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("nhan_viens_pkey", x => x.cccd);
                    table.ForeignKey(
                        name: "nhan_viens_id_pb_fkey",
                        column: x => x.id_pb,
                        principalTable: "phong_bans",
                        principalColumn: "id_pb",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "nhan_viens_id_tai_khoan_fkey",
                        column: x => x.id_tai_khoan,
                        principalTable: "tai_khoans",
                        principalColumn: "id_tai_khoan",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "quyet_dinh_nhan_sus",
                columns: table => new
                {
                    so_quyet_dinh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    loai_quyet_dinh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    id_bac_luong_moi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    id_chuc_vu_moi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ngay_hieu_luc = table.Column<DateOnly>(type: "date", nullable: false),
                    ngay_het_han = table.Column<DateOnly>(type: "date", nullable: true),
                    nguoi_ky = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("quyet_dinh_nhan_sus_pkey", x => x.so_quyet_dinh);
                    table.ForeignKey(
                        name: "quyet_dinh_nhan_sus_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "quyet_dinh_nhan_sus_id_bac_luong_moi_fkey",
                        column: x => x.id_bac_luong_moi,
                        principalTable: "bac_luongs",
                        principalColumn: "id_bac_luong",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tai_khoan_ngan_hangs",
                columns: table => new
                {
                    stk = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    chi_nhanh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ngay_mo_the = table.Column<DateOnly>(type: "date", nullable: true),
                    trang_thai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    id_ngan_hang = table.Column<Guid>(type: "uuid", nullable: true),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tai_khoan_ngan_hangs_pkey", x => x.stk);
                    table.ForeignKey(
                        name: "tai_khoan_ngan_hangs_cccd_fkey",
                        column: x => x.cccd,
                        principalTable: "nhan_viens",
                        principalColumn: "cccd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tai_khoan_ngan_hangs_id_ngan_hang_fkey",
                        column: x => x.id_ngan_hang,
                        principalTable: "ngan_hangs",
                        principalColumn: "id_ngan_hang",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_bac_luongs_id_chuc_vu",
                table: "bac_luongs",
                column: "id_chuc_vu");

            migrationBuilder.CreateIndex(
                name: "IX_nhan_viens_id_pb",
                table: "nhan_viens",
                column: "id_pb");

            migrationBuilder.CreateIndex(
                name: "IX_nhan_viens_id_tai_khoan",
                table: "nhan_viens",
                column: "id_tai_khoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "nhan_viens_email_key",
                table: "nhan_viens",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quyet_dinh_nhan_sus_cccd",
                table: "quyet_dinh_nhan_sus",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_quyet_dinh_nhan_sus_id_bac_luong_moi",
                table: "quyet_dinh_nhan_sus",
                column: "id_bac_luong_moi");

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_ngan_hangs_cccd",
                table: "tai_khoan_ngan_hangs",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoan_ngan_hangs_id_ngan_hang",
                table: "tai_khoan_ngan_hangs",
                column: "id_ngan_hang");

            migrationBuilder.CreateIndex(
                name: "IX_tai_khoans_id_vai_tro",
                table: "tai_khoans",
                column: "id_vai_tro");

            migrationBuilder.CreateIndex(
                name: "tai_khoans_ten_tai_khoan_key",
                table: "tai_khoans",
                column: "ten_tai_khoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tnhan_nviens_id_mqh",
                table: "tnhan_nviens",
                column: "id_mqh");

            migrationBuilder.CreateIndex(
                name: "IX_tnhan_nviens_ma_dinh_danh",
                table: "tnhan_nviens",
                column: "ma_dinh_danh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quyet_dinh_nhan_sus");

            migrationBuilder.DropTable(
                name: "tai_khoan_ngan_hangs");

            migrationBuilder.DropTable(
                name: "tnhan_nviens");

            migrationBuilder.DropTable(
                name: "bac_luongs");

            migrationBuilder.DropTable(
                name: "ngan_hangs");

            migrationBuilder.DropTable(
                name: "nhan_viens");

            migrationBuilder.DropTable(
                name: "moi_quan_hes");

            migrationBuilder.DropTable(
                name: "than_nhans");

            migrationBuilder.DropTable(
                name: "chuc_vus");

            migrationBuilder.DropTable(
                name: "phong_bans");

            migrationBuilder.DropTable(
                name: "tai_khoans");

            migrationBuilder.DropTable(
                name: "vai_tros");
        }
    }
}
