using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByNhanVien;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetPhieuKpiByKyKpi
{
    public class GetPhieuKpiByKyKpiQueryHandler : IRequestHandler<GetPhieuKpiByKyKpiQuery, Response<List<PhieuKpiDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IKpiAuthorizationService _kpiAuthorizationService;

        public GetPhieuKpiByKyKpiQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IKpiAuthorizationService kpiAuthorizationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _kpiAuthorizationService = kpiAuthorizationService;
        }

        public async Task<Response<List<PhieuKpiDto>>> Handle(GetPhieuKpiByKyKpiQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PhieuKpis
                .AsNoTracking()
                .Include(p => p.KyKpi)
                .Include(p => p.NhanVien)
                .Where(p => p.IdKyKpi == request.IdKyKpi)
                .AsQueryable();

            List<string> subordinateCccds = new List<string>();

            if (_currentUserService.UserId.HasValue)
            {
                var currentUserId = _currentUserService.UserId.Value;
                
                subordinateCccds = await _kpiAuthorizationService.GetSubordinateCccdsAsync(currentUserId, cancellationToken);
                
                var taiKhoan = await _context.TaiKhoans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.IdTaiKhoan == currentUserId, cancellationToken);

                if (subordinateCccds.Any())
                {
                    query = query.Where(p => subordinateCccds.Contains(p.CccdNhanVien));
                }
                else
                {
                    query = query.Where(p => false); // Manages no one
                }
            }

            query = query.OrderBy(p => p.NhanVien.HoTen);
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
                TenNhanVien = p.NhanVien.HoTen,
                CanManage = subordinateCccds.Contains(p.CccdNhanVien)
            }).ToList();

            return new Response<List<PhieuKpiDto>>(result);
        }
    }
}

