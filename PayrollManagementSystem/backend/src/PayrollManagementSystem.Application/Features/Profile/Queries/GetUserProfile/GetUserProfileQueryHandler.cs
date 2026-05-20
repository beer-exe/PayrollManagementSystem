using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Profile.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

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
            var profile = await _context.NhanViens
                .Where(nv => nv.IdTaiKhoan == request.TaiKhoanId)
                .Select(nv => new UserProfileDto
                {
                    Cccd = nv.Cccd,
                    HoTen = nv.HoTen,
                    GioiTinh = nv.GioiTinh,
                    Sdt = nv.Sdt,
                    Email = nv.Email,
                    NgaySinh = nv.NgaySinh,
                    DanToc = nv.DanToc,
                    DiaChi = nv.DiaChi,
                    ChuyenNganh = nv.ChuyenNganh,
                    NgayVaoLam = nv.NgayVaoLam,
                    TrangThai = nv.TrangThai.ToString(),
                    SoBhxh = nv.SoBhxh,
                    SoBhyt = nv.SoBhyt,
                    TenPhongBan = nv.PhongBan != null ? nv.PhongBan.TenPb : null,

                    TenChucVu = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .Select(qd => _context.ChucVus.FirstOrDefault(cv => cv.IdChucVu == qd.IdChucVuMoi).TenChucVu)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                throw new ApiException("Không tìm thấy thông tin hồ sơ cho tài khoản này.");
            }

            return new Response<UserProfileDto>(profile, "Lấy thông tin hồ sơ thành công.");
        }
    }
}