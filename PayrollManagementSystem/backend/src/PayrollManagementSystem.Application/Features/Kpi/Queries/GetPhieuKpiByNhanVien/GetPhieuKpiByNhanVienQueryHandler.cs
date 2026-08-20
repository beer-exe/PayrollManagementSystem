using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByNhanVien
{
    public class GetPhieuKpiByNhanVienQueryHandler : IRequestHandler<GetPhieuKpiByNhanVienQuery, Response<List<PhieuKpiDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPhieuKpiByNhanVienQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<PhieuKpiDto>>> Handle(GetPhieuKpiByNhanVienQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PhieuKpis
                .AsNoTracking()
                .Include(p => p.KyKpi)
                .Include(p => p.NhanVien)
                .Where(p => p.NhanVien.IdTaiKhoan == request.TaiKhoanId)
                .OrderByDescending(p => p.KyKpi.Nam)
                .ThenByDescending(p => p.KyKpi.Thang);

            var list = await query.ToListAsync(cancellationToken);

            var result = list.Select(p => new PhieuKpiDto
            {
                IdPhieuKpi = p.IdPhieuKpi,
                IdKyKpi = p.IdKyKpi,
                TenKyKpi = p.KyKpi.TenKyKpi,
                Thang = p.KyKpi.Thang,
                Nam = p.KyKpi.Nam,
                TongDiemKpi = p.TongDiemKpi,
                HeSoP3 = p.HeSoP3,
                TrangThaiValue = (int)p.TrangThai,
                TrangThai = p.TrangThai.GetDescription(),
                NhanXet = p.NhanXet,
                CccdNhanVien = p.CccdNhanVien,
                TenNhanVien = p.NhanVien.HoTen
            }).ToList();

            return new Response<List<PhieuKpiDto>>(result);
        }
    }
}

