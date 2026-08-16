using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetKyKpiList
{
    public class GetKyKpiListQueryHandler : IRequestHandler<GetKyKpiListQuery, Response<List<KyKpiDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetKyKpiListQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<KyKpiDto>>> Handle(GetKyKpiListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.KyKpis
                .AsNoTracking()
                .Include(x => x.PhieuKpis)
                .OrderByDescending(x => x.Nam)
                .ThenByDescending(x => x.Thang)
                .AsQueryable();

            var list = await query.ToListAsync(cancellationToken);

            var result = list.Select(k => new KyKpiDto
            {
                IdKyKpi = k.IdKyKpi,
                TenKyKpi = k.TenKyKpi,
                Thang = k.Thang,
                Nam = k.Nam,
                TrangThaiValue = (int)k.TrangThai,
                TrangThai = k.TrangThai.GetDescription(),
                TongSoPhieu = k.PhieuKpis.Count,
                SoPhieuDaDuyet = k.PhieuKpis.Count(p => p.TrangThai == TrangThaiPhieuKpi.DA_PHE_DUYET)
            }).ToList();

            return new Response<List<KyKpiDto>>(result);
        }
    }
}

