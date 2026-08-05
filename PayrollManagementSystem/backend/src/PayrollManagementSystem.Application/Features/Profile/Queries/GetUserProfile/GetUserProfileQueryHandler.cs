using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Profile.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Profile.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Response<UserProfileDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserProfileQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var profileData = await _context.NhanViens
                .AsNoTracking()
                .Where(nv => nv.IdTaiKhoan == request.TaiKhoanId)
                .Select(nv => new 
                {
                    Cccd = nv.Cccd,
                    HoTen = nv.HoTen,
                    GioiTinh = nv.GioiTinh,
                    Sdt = nv.Sdt,
                    Email = nv.Email,
                    UserAvatar = _context.TaiKhoans.Where(t => t.IdTaiKhoan == nv.IdTaiKhoan).Select(t => t.UserAvatar).FirstOrDefault(),
                    NgaySinh = nv.NgaySinh,
                    DanToc = nv.DanToc,
                    DiaChi = nv.DiaChi,
                    ChuyenNganh = nv.ChuyenNganh,
                    NgayVaoLam = nv.NgayVaoLam,
                    TrangThai = nv.TrangThai,
                    SoBhxh = nv.SoBhxh,
                    SoBhyt = nv.SoBhyt,
                    TenPhongBan = nv.PhongBan != null ? nv.PhongBan.TenPb : null,

                    TenChucVu = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC && qd.NgayHieuLuc <= today)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .ThenByDescending(qd => qd.CreatedAt)
                        .Select(qd => _context.ChucVus.FirstOrDefault(cv => cv.IdChucVu == qd.IdChucVuMoi).TenChucVu)
                        .FirstOrDefault(),

                    SoTaiKhoan = nv.SoTaiKhoan,
                    TenNganHang = nv.TenNganHang,
                    MaSoThue = nv.MaSoThue,
                    HeSoP2 = nv.HeSoP2,
                    IdPb = nv.IdPb,

                    LuongP1 = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC && qd.NgayHieuLuc <= today)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .ThenByDescending(qd => qd.CreatedAt)
                        .Select(qd => (decimal?)_context.BacLuongs.FirstOrDefault(bl => bl.IdBacLuong == qd.IdBacLuongMoi).LuongP1)
                        .FirstOrDefault(),

                    SoHopDong = _context.HopDongLaoDongs
                        .Where(hd => hd.Cccd == nv.Cccd && hd.TrangThai == TrangThaiHopDong.HIEU_LUC)
                        .OrderByDescending(hd => hd.NgayBatDau)
                        .Select(hd => hd.SoHopDong)
                        .FirstOrDefault(),

                    LoaiHopDong = _context.HopDongLaoDongs
                        .Where(hd => hd.Cccd == nv.Cccd && hd.TrangThai == TrangThaiHopDong.HIEU_LUC)
                        .OrderByDescending(hd => hd.NgayBatDau)
                        .Select(hd => hd.LoaiHopDong)
                        .FirstOrDefault(),

                    NgayBatDauHopDong = _context.HopDongLaoDongs
                        .Where(hd => hd.Cccd == nv.Cccd && hd.TrangThai == TrangThaiHopDong.HIEU_LUC)
                        .OrderByDescending(hd => hd.NgayBatDau)
                        .Select(hd => hd.NgayBatDau)
                        .FirstOrDefault(),

                    ThanNhans = _context.TNhanNviens
                        .Where(tnnv => tnnv.Cccd == nv.Cccd)
                        .Select(tnnv => new ProfileThanNhanDto
                        {
                            MaDinhDanh = tnnv.MaDinhDanh,
                            TenTn = tnnv.ThanNhan.TenTn,
                            NgaySinh = tnnv.ThanNhan.NgaySinh,
                            MoiQuanHe = tnnv.MoiQuanHe != null ? tnnv.MoiQuanHe.TenQuanHe : null
                        }).ToList(),

                    LichSuCongTac = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .ThenByDescending(qd => qd.CreatedAt)
                        .Select(qd => new 
                        {
                            SoQuyetDinh = qd.SoQuyetDinh,
                            LoaiQuyetDinh = qd.LoaiQuyetDinh,
                            NgayHieuLuc = qd.NgayHieuLuc,
                            TenPhongBanMoi = _context.ChucVus.Where(cv => cv.IdChucVu == qd.IdChucVuMoi).Select(cv => cv.PhongBan.TenPb).FirstOrDefault(),
                            TenChucVuMoi = _context.ChucVus.FirstOrDefault(cv => cv.IdChucVu == qd.IdChucVuMoi).TenChucVu,
                            LuongP1Moi = (decimal?)_context.BacLuongs.FirstOrDefault(bl => bl.IdBacLuong == qd.IdBacLuongMoi).LuongP1,
                            TrangThai = qd.TrangThai
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (profileData == null)
            {
                throw new ApiException("Không tìm thấy thông tin hồ sơ cho tài khoản này.");
            }

            var profile = new UserProfileDto
            {
                Cccd = profileData.Cccd,
                HoTen = profileData.HoTen,
                GioiTinh = profileData.GioiTinh,
                Sdt = profileData.Sdt,
                Email = profileData.Email,
                UserAvatar = profileData.UserAvatar,
                NgaySinh = profileData.NgaySinh,
                DanToc = profileData.DanToc,
                DiaChi = profileData.DiaChi,
                ChuyenNganh = profileData.ChuyenNganh,
                NgayVaoLam = profileData.NgayVaoLam,
                TrangThai = profileData.TrangThai?.GetDescription(),
                SoBhxh = profileData.SoBhxh,
                SoBhyt = profileData.SoBhyt,
                TenPhongBan = profileData.TenPhongBan,
                TenChucVu = profileData.TenChucVu,
                SoTaiKhoan = profileData.SoTaiKhoan,
                TenNganHang = profileData.TenNganHang,
                MaSoThue = profileData.MaSoThue,
                HeSoP2 = profileData.HeSoP2,
                IdPb = profileData.IdPb,
                LuongP1 = profileData.LuongP1,
                SoHopDong = profileData.SoHopDong,
                LoaiHopDong = profileData.LoaiHopDong,
                NgayBatDauHopDong = profileData.NgayBatDauHopDong,
                ThanNhans = profileData.ThanNhans,
                LichSuCongTac = profileData.LichSuCongTac.Select(ls => new LichSuCongTacDto
                {
                    SoQuyetDinh = ls.SoQuyetDinh,
                    LoaiQuyetDinh = ls.LoaiQuyetDinh,
                    NgayHieuLuc = ls.NgayHieuLuc,
                    TenPhongBanMoi = ls.TenPhongBanMoi,
                    TenChucVuMoi = ls.TenChucVuMoi,
                    LuongP1Moi = ls.LuongP1Moi,
                    TrangThai = ls.TrangThai.GetDescription()
                }).ToList()
            };

            return new Response<UserProfileDto>(profile, "Lấy thông tin hồ sơ thành công.");
        }
    }
}