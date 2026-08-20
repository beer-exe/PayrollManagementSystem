using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetMyPayroll
{
    public class GetMyPayrollQueryHandler : IRequestHandler<GetMyPayrollQuery, Response<List<MyPayrollDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyPayrollQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Response<List<MyPayrollDto>>> Handle(GetMyPayrollQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            if (userId == Guid.Empty)
            {
                throw new ApiException("Không tìm thấy thông tin người dùng.");
            }

            var nhanVien = await _context.NhanViens
                .AsNoTracking()
                .Include(n => n.PhongBan)
                .FirstOrDefaultAsync(n => n.IdTaiKhoan == userId, cancellationToken);

            if (nhanVien == null)
            {
                throw new ApiException("Không tìm thấy thông tin nhân viên.");
            }

            var bangLuongs = await _context.BangLuongs
                .Include(b => b.NhanVien)
                .Include(b => b.KyLuong)
                .Where(b => b.CccdNhanVien == nhanVien.Cccd && b.Nam == request.Nam)
                .OrderByDescending(b => b.Thang)
                .ToListAsync(cancellationToken);

            var quyetDinhs = await _context.QuyetDinhNhanSus
                .Include(x => x.ChucVuMoi)
                    .ThenInclude(cv => cv.PhongBan)
                .Where(x => x.Cccd == nhanVien.Cccd && x.TrangThai != Domain.Enums.TrangThaiQuyetDinh.HUY_BO)
                .ToListAsync(cancellationToken);

            var result = bangLuongs.Select(b =>
            {
                var endOfMonth = new DateOnly(request.Nam, b.Thang, DateTime.DaysInMonth(request.Nam, b.Thang));
                var startOfMonth = new DateOnly(request.Nam, b.Thang, 1);
                var qd = quyetDinhs
                    .Where(x => x.NgayHieuLuc <= endOfMonth
                             && (x.NgayHetHan == null || x.NgayHetHan >= startOfMonth))
                    .OrderByDescending(x => x.NgayHieuLuc)
                    .FirstOrDefault();

                return new MyPayrollDto
                {
                    IdBangLuong = b.IdBangLuong,
                    IdKyLuong = b.IdKyLuong,
                    CccdNhanVien = b.CccdNhanVien,
                    TenNhanVien = nhanVien.HoTen,
                    TenPhongBan = qd?.ChucVuMoi?.PhongBan?.TenPb ?? (nhanVien.PhongBan?.TenPb ?? ""),
                    TenChucVu = qd?.ChucVuMoi?.TenChucVu ?? "",

                    Thang = b.Thang,
                    Nam = b.Nam,

                    P1 = b.P1,
                    HeSoP2 = b.HeSoP2,
                    HeSoP3 = b.HeSoP3,

                    NgayCongChuan = b.NgayCongChuan,
                    NgayCongThucTe = b.NgayCongThucTe,
                    GioCongChuan = b.GioCongChuan,
                    GioCongThucTe = b.GioCongThucTe,

                    LuongThoiGian = b.LuongThoiGian,
                    LuongHieuSuatP3 = b.LuongHieuSuatP3,

                    PhuCap = b.PhuCap,
                    Thuong = b.Thuong,
                    TangCa = b.TangCa,

                    Phat = b.Phat,
                    KhauTru = b.KhauTru,
                    TruThue = b.TruThue,

                    TongThuNhap = b.TongThuNhap,
                    ThucLinh = b.ThucLinh,

                    GhiChu = b.GhiChu,
                    ChiTietKhauTru = b.ChiTietKhauTru,
                    ChiTietThue = b.ChiTietThue,

                    TrangThaiKyLuong = b.KyLuong.TrangThai.GetDescription(),
                    TrangThai = b.TrangThai.ToString(),
                    TrangThaiText = b.TrangThai.GetDescription(),
                    LyDoKhieuNai = b.LyDoKhieuNai,
                    PhanHoiKhieuNai = b.PhanHoiKhieuNai
                };
            }).ToList();

            return new Response<List<MyPayrollDto>>(result, "Lấy thông tin bảng lương cá nhân thành công.");
        }
    }
}
