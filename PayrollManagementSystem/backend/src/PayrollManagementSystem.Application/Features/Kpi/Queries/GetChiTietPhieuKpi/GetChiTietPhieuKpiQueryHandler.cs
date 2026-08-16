using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Queries.GetChiTietPhieuKpi
{
    public class GetChiTietPhieuKpiQueryHandler : IRequestHandler<GetChiTietPhieuKpiQuery, Response<PhieuKpiDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IKpiAuthorizationService _kpiAuthorizationService;

        public GetChiTietPhieuKpiQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IKpiAuthorizationService kpiAuthorizationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _kpiAuthorizationService = kpiAuthorizationService;
        }

        public async Task<Response<PhieuKpiDetailDto>> Handle(GetChiTietPhieuKpiQuery request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuKpis
                .AsNoTracking()
                .Include(p => p.KyKpi)
                .Include(p => p.NhanVien)
                .Include(p => p.QuanLy)
                .Include(p => p.ChiTietKpis)
                .FirstOrDefaultAsync(p => p.IdPhieuKpi == request.IdPhieuKpi, cancellationToken);

            if (phieu == null)
                throw new ApiException($"Không tìm thấy phiếu KPI {request.IdPhieuKpi}");

            bool canManage = false;
            if (_currentUserService.UserId.HasValue)
            {
                canManage = await _kpiAuthorizationService.CanManageAsync(_currentUserService.UserId.Value, phieu.CccdNhanVien, cancellationToken);
            }

            var result = new PhieuKpiDetailDto
            {
                IdPhieuKpi = phieu.IdPhieuKpi,
                IdKyKpi = phieu.IdKyKpi,
                TenKyKpi = phieu.KyKpi.TenKyKpi,
                Thang = phieu.KyKpi.Thang,
                Nam = phieu.KyKpi.Nam,
                CccdNhanVien = phieu.CccdNhanVien,
                TenNhanVien = phieu.NhanVien.HoTen,
                CccdQuanLy = phieu.CccdQuanLy,
                TenQuanLy = phieu.QuanLy?.HoTen,
                TongDiemKpi = phieu.TongDiemKpi,
                HeSoP3 = phieu.HeSoP3,
                NhanXet = phieu.NhanXet,
                TrangThaiValue = (int)phieu.TrangThai,
                TrangThai = phieu.TrangThai.GetDescription(),
                CanManage = canManage,
                ChiTietKpis = phieu.ChiTietKpis.Select(c => new ChiTietKpiDto
                {
                    IdChiTietKpi = c.IdChiTietKpi,
                    IdPhieuKpi = c.IdPhieuKpi,
                    MucTieu = c.MucTieu,
                    DonViTinh = c.DonViTinh,
                    TrongSo = c.TrongSo,
                    ChiTieu = c.ChiTieu,
                    ThucTe = c.ThucTe,
                    TiLeHoanThanh = c.TiLeHoanThanh,
                    DiemKpi = c.DiemKpi
                }).ToList()
            };

            return new Response<PhieuKpiDetailDto>(result);
        }
    }
}

