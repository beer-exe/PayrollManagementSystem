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
                .OrderByDescending(nv => nv.NgayVaoLam) // Sắp xếp nhân viên mới nhất lên đầu
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
                    TenPhongBan = nv.PhongBan != null ? nv.PhongBan.TenPb : null,

                    TenChucVu = _context.QuyetDinhNhanSus
                        .Where(qd => qd.Cccd == nv.Cccd && qd.TrangThai == TrangThaiQuyetDinh.HIEU_LUC)
                        .OrderByDescending(qd => qd.NgayHieuLuc)
                        .Select(qd => _context.ChucVus.FirstOrDefault(cv => cv.IdChucVu == qd.IdChucVuMoi).TenChucVu)
                        .FirstOrDefault(),

                    ThanNhans = _context.TNhanNviens
                        .Where(tn => tn.Cccd == nv.Cccd)
                        .Select(tn => new ThanNhanDto
                        {
                            TenTn = tn.ThanNhan.TenTn,
                            NgaySinh = tn.ThanNhan.NgaySinh.HasValue ? tn.ThanNhan.NgaySinh.Value.ToString("yyyy-MM-dd") : null,
                            MoiQuanHe = tn.MoiQuanHe != null ? tn.MoiQuanHe.TenQuanHe : "Khác"
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