using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Employees.DTOs;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.ExportEmployees
{
    public class ExportEmployeesQueryHandler : IRequestHandler<ExportEmployeesQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;

        public ExportEmployeesQueryHandler(IApplicationDbContext context, IExcelService excelService)
        {
            _context = context;
            _excelService = excelService;
        }

        public async Task<byte[]> Handle(ExportEmployeesQuery request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var query = _context.NhanViens
                .Include(nv => nv.PhongBan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = request.SearchTerm.Trim().ToLower();
                query = query.Where(nv =>
                    nv.Cccd.ToLower().Contains(search) ||
                    nv.HoTen.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.IdPb))
            {
                query = query.Where(nv => nv.IdPb == request.IdPb);
            }

            var employeesData = await query
                .AsNoTracking()
                .OrderByDescending(nv => nv.NgayVaoLam)
                .Select(nv => new
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

                    LuongP1 = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC && qd.NgayHieuLuc <= today)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .ThenByDescending(qd => qd.CreatedAt)
                        .Select(qd => _context.BacLuongs.FirstOrDefault(bl => bl.IdBacLuong == qd.IdBacLuongMoi).LuongP1)
                        .FirstOrDefault(),

                    HeSoP2 = _context.PhieuDanhGiaNangLucs
                        .Where(pdg => pdg.CccdNhanVien == nv.Cccd && pdg.TrangThai == TrangThaiPhieuDanhGia.DA_HOAN_THANH)
                        .OrderByDescending(pdg => pdg.KyDanhGia.NgayKetThuc)
                        .Select(pdg => pdg.HeSoP2)
                        .FirstOrDefault() ?? 1.00m
                })
                .ToListAsync(cancellationToken);

            var employees = employeesData.Select(nv => new EmployeeDto
            {
                Cccd = nv.Cccd,
                HoTen = nv.HoTen,
                GioiTinh = nv.GioiTinh,
                Sdt = nv.Sdt,
                Email = nv.Email,
                NgaySinh = nv.NgaySinh.HasValue ? nv.NgaySinh.Value.ToString("yyyy-MM-dd") : null,
                DanToc = nv.DanToc,
                DiaChi = nv.DiaChi,
                ChuyenNganh = nv.ChuyenNganh,
                NgayVaoLam = nv.NgayVaoLam.HasValue ? nv.NgayVaoLam.Value.ToString("yyyy-MM-dd") : null,
                TrangThai = nv.TrangThai?.GetDescription(),
                SoBhxh = nv.SoBhxh,
                SoBhyt = nv.SoBhyt,
                TenPhongBan = nv.TenPhongBan,
                TenChucVu = nv.TenChucVu,
                SoTaiKhoan = nv.SoTaiKhoan,
                TenNganHang = nv.TenNganHang,
                MaSoThue = nv.MaSoThue,
                LuongP1 = nv.LuongP1,
                HeSoP2 = nv.HeSoP2
            }).ToList();

            return _excelService.ExportEmployeesToExcel(employees);
        }
    }
}
