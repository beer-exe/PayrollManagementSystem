using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Application.Features.Kpi.Commands.SaveChiTietKpi;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.AssignPhieuKpi
{
    public class AssignPhieuKpiCommandHandler : IRequestHandler<AssignPhieuKpiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IKpiAuthorizationService _kpiAuthorizationService;

        public AssignPhieuKpiCommandHandler(IApplicationDbContext context, IKpiAuthorizationService kpiAuthorizationService)
        {
            _context = context;
            _kpiAuthorizationService = kpiAuthorizationService;
        }

        public async Task<Response<Guid>> Handle(AssignPhieuKpiCommand request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuKpis
                .Include(p => p.ChiTietKpis)
                .FirstOrDefaultAsync(p => p.IdPhieuKpi == request.IdPhieuKpi, cancellationToken);

            if (phieu == null)
                throw new ApiException($"Không tìm thấy phiếu KPI {request.IdPhieuKpi}");

            if (phieu.TrangThai != TrangThaiPhieuKpi.CHO_GIAO_MUC_TIEU)
                throw new ApiException("Phiếu KPI không ở trạng thái chờ giao mục tiêu.");

            bool canManage = await _kpiAuthorizationService.CanManageAsync(request.TaiKhoanIdQuanLy, phieu.CccdNhanVien, cancellationToken);
            if (!canManage)
            {
                throw new ApiException("Bạn không có quyền giao KPI cho nhân viên này do không phải quản lý trực tiếp.");
            }

            var quanLy = await _context.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdTaiKhoan == request.TaiKhoanIdQuanLy, cancellationToken);
                
            if (quanLy == null) throw new ApiException("Không tìm thấy thông tin quản lý.");

            phieu.CccdQuanLy = quanLy.Cccd;

            var incomingIds = request.ChiTietKpis.Where(x => x.IdChiTietKpi.HasValue).Select(x => x.IdChiTietKpi!.Value).ToList();
            var toRemove = phieu.ChiTietKpis.Where(x => !incomingIds.Contains(x.IdChiTietKpi)).ToList();
            _context.ChiTietKpis.RemoveRange(toRemove);

            foreach (var item in request.ChiTietKpis)
            {
                if (item.IdChiTietKpi.HasValue)
                {
                    var existing = phieu.ChiTietKpis.FirstOrDefault(x => x.IdChiTietKpi == item.IdChiTietKpi.Value);
                    if (existing != null)
                    {
                        existing.MucTieu = item.MucTieu;
                        existing.DonViTinh = item.DonViTinh;
                        existing.TrongSo = item.TrongSo;
                        existing.ChiTieu = item.ChiTieu;
                        existing.ThucTe = 0; // Ép về 0 khi giao
                        existing.TiLeHoanThanh = 0;
                        existing.DiemKpi = 0;
                        existing.LoaiTieuChi = item.LoaiTieuChi;
                    }
                }
                else
                {
                    _context.ChiTietKpis.Add(new ChiTietKpi
                    {
                        IdChiTietKpi = Guid.NewGuid(),
                        IdPhieuKpi = phieu.IdPhieuKpi,
                        MucTieu = item.MucTieu,
                        DonViTinh = item.DonViTinh,
                        TrongSo = item.TrongSo,
                        ChiTieu = item.ChiTieu,
                        ThucTe = 0,
                        TiLeHoanThanh = 0,
                        DiemKpi = 0,
                        LoaiTieuChi = item.LoaiTieuChi
                    });
                }
            }

            phieu.TongDiemKpi = 0;
            phieu.HeSoP3 = 0;
            phieu.TrangThai = TrangThaiPhieuKpi.DANG_THUC_HIEN;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(phieu.IdPhieuKpi, "Giao mục tiêu KPI thành công.");
        }
    }
}

