using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.ApprovePhieuKpi
{
    public class ApprovePhieuKpiCommandHandler : IRequestHandler<ApprovePhieuKpiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IKpiAuthorizationService _kpiAuthorizationService;

        public ApprovePhieuKpiCommandHandler(IApplicationDbContext context, IKpiAuthorizationService kpiAuthorizationService)
        {
            _context = context;
            _kpiAuthorizationService = kpiAuthorizationService;
        }

        public async Task<Response<Guid>> Handle(ApprovePhieuKpiCommand request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuKpis
                .Include(p => p.ChiTietKpis)
                .FirstOrDefaultAsync(p => p.IdPhieuKpi == request.IdPhieuKpi, cancellationToken);

            if (phieu == null)
                throw new ApiException($"Không tìm thấy phiếu KPI {request.IdPhieuKpi}");

            if (phieu.TrangThai != TrangThaiPhieuKpi.CHO_PHE_DUYET && phieu.TrangThai != TrangThaiPhieuKpi.DANG_THUC_HIEN)
                throw new ApiException("Phiếu KPI phải ở trạng thái Chờ phê duyệt hoặc Đang thực hiện.");

            bool canManage = await _kpiAuthorizationService.CanManageAsync(request.TaiKhoanIdQuanLy, phieu.CccdNhanVien, cancellationToken);
            if (!canManage)
            {
                throw new ApiException("Bạn không có quyền phê duyệt KPI cho nhân viên này do không phải quản lý trực tiếp.");
            }

            var quanLy = await _context.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.IdTaiKhoan == request.TaiKhoanIdQuanLy, cancellationToken);

            if (quanLy == null) throw new ApiException("Không tìm thấy thông tin quản lý.");

            phieu.TrangThai = TrangThaiPhieuKpi.DA_PHE_DUYET;
            phieu.CccdQuanLy = quanLy.Cccd;
            phieu.NhanXet = request.NhanXet;

            decimal tongDiem = phieu.ChiTietKpis.Sum(x => x.DiemKpi);
            phieu.TongDiemKpi = tongDiem;
            phieu.HeSoP3 = tongDiem / 100m;
            if (phieu.HeSoP3 < 0) phieu.HeSoP3 = 0;

            await _context.SaveChangesAsync(cancellationToken);

            var kyKpi = await _context.KyKpis
                .Include(k => k.PhieuKpis)
                .FirstOrDefaultAsync(k => k.IdKyKpi == phieu.IdKyKpi, cancellationToken);

            if (kyKpi != null && kyKpi.PhieuKpis.All(p => p.TrangThai == TrangThaiPhieuKpi.DA_PHE_DUYET))
            {
                kyKpi.TrangThai = TrangThaiKyKpi.DA_CHOT;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new Response<Guid>(phieu.IdPhieuKpi, "Phê duyệt phiếu KPI thành công.");
        }
    }
}

