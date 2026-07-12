using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "vai_tros",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "vai_tros",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "vai_tros",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "vai_tros",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "vai_tros",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "than_nhans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "than_nhans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "than_nhans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "than_nhans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "than_nhans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "than_nhan_nhan_vien",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "than_nhan_nhan_vien",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "than_nhan_nhan_vien",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "than_nhan_nhan_vien",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "than_nhan_nhan_vien",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "tai_khoans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "tai_khoans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "tai_khoans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "tai_khoans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "tai_khoans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "quyet_dinh_nhan_sus",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "quyet_dinh_nhan_sus",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "quyet_dinh_nhan_sus",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "quyet_dinh_nhan_sus",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "quyet_dinh_nhan_sus",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "phong_bans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "phong_bans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "phong_bans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "phong_bans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "phong_bans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "phieu_danh_gia_nang_lucs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "phieu_danh_gia_nang_lucs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "phieu_danh_gia_nang_lucs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "phieu_danh_gia_nang_lucs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "phieu_danh_gia_nang_lucs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "nhat_ky_trang_thais",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "nhat_ky_trang_thais",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "nhat_ky_trang_thais",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "nhat_ky_trang_thais",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "nhat_ky_trang_thais",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "nhan_viens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "nhan_viens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "nhan_viens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "nhan_viens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "nhan_viens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ngach_luongs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ngach_luongs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ngach_luongs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ngach_luongs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ngach_luongs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "muc_quy_doi_p2s",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "muc_quy_doi_p2s",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "muc_quy_doi_p2s",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "muc_quy_doi_p2s",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "muc_quy_doi_p2s",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "moi_quan_hes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "moi_quan_hes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "moi_quan_hes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "moi_quan_hes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "moi_quan_hes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ky_danh_gias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ky_danh_gias",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ky_danh_gias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ky_danh_gias",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "ky_danh_gias",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "khung_nang_luc_p2",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "khung_nang_luc_p2",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "khung_nang_luc_p2",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "khung_nang_luc_p2",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "khung_nang_luc_p2",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "hop_dong_lao_dongs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "hop_dong_lao_dongs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "hop_dong_lao_dongs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "hop_dong_lao_dongs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "hop_dong_lao_dongs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "chuc_vus",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "chuc_vus",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "chuc_vus",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "chuc_vus",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "chuc_vus",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "chi_tiet_danh_gia_nang_lucs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "chi_tiet_danh_gia_nang_lucs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "chi_tiet_danh_gia_nang_lucs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "chi_tiet_danh_gia_nang_lucs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "chi_tiet_danh_gia_nang_lucs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "bac_luongs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "bac_luongs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "bac_luongs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "bac_luongs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "bac_luongs",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "vai_tros");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "vai_tros");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "vai_tros");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "vai_tros");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "vai_tros");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "than_nhans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "than_nhans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "than_nhans");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "than_nhans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "than_nhans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tai_khoans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "tai_khoans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "tai_khoans");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "tai_khoans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "tai_khoans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "phong_bans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "phong_bans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "phong_bans");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "phong_bans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "phong_bans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "nhan_viens");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ngach_luongs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ngach_luongs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ngach_luongs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ngach_luongs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ngach_luongs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "moi_quan_hes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "moi_quan_hes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "moi_quan_hes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "moi_quan_hes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "moi_quan_hes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ky_danh_gias");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ky_danh_gias");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ky_danh_gias");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ky_danh_gias");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ky_danh_gias");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "khung_nang_luc_p2");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "khung_nang_luc_p2");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "khung_nang_luc_p2");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "khung_nang_luc_p2");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "khung_nang_luc_p2");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "chuc_vus");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "bac_luongs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "bac_luongs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "bac_luongs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "bac_luongs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "bac_luongs");
        }
    }
}
