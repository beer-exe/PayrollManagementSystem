using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetPayrollList
{
    public class GetPayrollListQueryHandler : IRequestHandler<GetPayrollListQuery, Response<List<PayrollListDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPayrollListQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<PayrollListDto>>> Handle(GetPayrollListQuery request, CancellationToken cancellationToken)
        {
            var bangLuongs = await _context.BangLuongs
                .Include(x => x.NhanVien)
                    .ThenInclude(n => n.PhongBan)
                .Where(x => x.Thang == request.Thang && x.Nam == request.Nam)
                .ToListAsync(cancellationToken);

            // Cần lấy thêm Chức vụ hiện tại (hoặc chức vụ tại thời điểm tính lương). Tạm lấy Quyết định nhân sự mới nhất
            var cccdList = bangLuongs.Select(x => x.CccdNhanVien).ToList();
            var quyetDinhs = await _context.QuyetDinhNhanSus
                .Include(x => x.ChucVuMoi)
                .Where(x => cccdList.Contains(x.Cccd) && x.TrangThai == Domain.Enums.TrangThaiQuyetDinh.HIEU_LUC)
                .ToListAsync(cancellationToken);

            var result = bangLuongs.Select(bl => 
            {
                var qd = quyetDinhs
                    .Where(x => x.Cccd == bl.CccdNhanVien && x.NgayHieuLuc <= new DateOnly(request.Nam, request.Thang, DateTime.DaysInMonth(request.Nam, request.Thang)))
                    .OrderByDescending(x => x.NgayHieuLuc)
                    .FirstOrDefault();

                return new PayrollListDto
                {
                    IdBangLuong = bl.IdBangLuong,
                    IdKyLuong = bl.IdKyLuong,
                    CccdNhanVien = bl.CccdNhanVien,
                    TenNhanVien = bl.NhanVien?.HoTen ?? "",
                    TenPhongBan = bl.NhanVien?.PhongBan?.TenPb ?? "",
                    TenChucVu = qd?.ChucVuMoi?.TenChucVu ?? "",
                    Thang = bl.Thang,
                    Nam = bl.Nam,
                    P1 = bl.P1,
                    HeSoP2 = bl.HeSoP2,
                    HeSoP3 = bl.HeSoP3,
                    NgayCongChuan = bl.NgayCongChuan,
                    NgayCongThucTe = bl.NgayCongThucTe,
                    LuongThoiGian = bl.LuongThoiGian,
                    LuongHieuSuatP3 = bl.LuongHieuSuatP3,
                    PhuCap = bl.PhuCap,
                    Thuong = bl.Thuong,
                    TangCa = bl.TangCa,
                    Phat = bl.Phat,
                    TruBaoHiem = bl.TruBaoHiem,
                    TruThue = bl.TruThue,
                    TongThuNhap = bl.TongThuNhap,
                    ThucLinh = bl.ThucLinh,
                    GhiChu = bl.GhiChu
                };
            }).OrderBy(x => x.TenPhongBan).ThenBy(x => x.TenNhanVien).ToList();

            return new Response<List<PayrollListDto>>(result);
        }
    }
}
