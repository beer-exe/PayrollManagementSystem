using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Employees.DTOs;
using PayrollManagementSystem.Application.Features.Profile.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.GetEmployees
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResponse<IEnumerable<EmployeeDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmployeesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<IEnumerable<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
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

            int totalRecords = await query.CountAsync(cancellationToken);

            var employees = await query
                .OrderByDescending(nv => nv.NgayVaoLam)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(nv => new EmployeeDto
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
                    TrangThai = nv.TrangThai.ToString(),
                    SoBhxh = nv.SoBhxh,
                    SoBhyt = nv.SoBhyt,
                    IdPb = nv.IdPb ?? _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC && qd.NgayHieuLuc <= today)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .ThenByDescending(qd => qd.CreatedAt)
                        .Select(qd => _context.ChucVus.FirstOrDefault(cv => cv.IdChucVu == qd.IdChucVuMoi).IdPhongBan)
                        .FirstOrDefault(),

                    TenPhongBan = nv.PhongBan != null ? nv.PhongBan.TenPb : _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC && qd.NgayHieuLuc <= today)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .ThenByDescending(qd => qd.CreatedAt)
                        .Select(qd => _context.ChucVus.Where(cv => cv.IdChucVu == qd.IdChucVuMoi)
                            .Select(cv => cv.PhongBan.TenPb).FirstOrDefault())
                        .FirstOrDefault(),

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
                        .FirstOrDefault() ?? 1.00m,
                        
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
                        .Select(hd => hd.NgayBatDau.ToString("yyyy-MM-dd"))
                        .FirstOrDefault(),

                    ThanNhans = _context.TNhanNviens
                        .Where(tn => tn.Cccd == nv.Cccd)
                        .Select(tn => new ThanNhanDto
                        {
                            MaDinhDanh = tn.MaDinhDanh,
                            TenTn = tn.ThanNhan.TenTn,
                            NgaySinh = tn.ThanNhan.NgaySinh.HasValue ? tn.ThanNhan.NgaySinh.Value.ToString("yyyy-MM-dd") : null,
                            IdMqh = tn.IdMqh,
                            MoiQuanHe = tn.MoiQuanHe != null ? tn.MoiQuanHe.TenQuanHe : "Khác",
                            LaNguoiPhuThuoc = tn.LaNguoiPhuThuoc
                        }).ToList() ?? new List<ThanNhanDto>()
                })
                .ToListAsync(cancellationToken);

            return new PagedResponse<IEnumerable<EmployeeDto>>(
                employees,
                request.PageNumber,
                request.PageSize,
                totalRecords,
                "Lấy danh sách nhân viên thành công.");
        }
    }
}