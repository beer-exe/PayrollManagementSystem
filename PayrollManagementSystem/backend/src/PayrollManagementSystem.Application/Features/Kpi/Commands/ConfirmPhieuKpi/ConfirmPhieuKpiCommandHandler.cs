using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.ConfirmPhieuKpi
{
    public class ConfirmPhieuKpiCommandHandler : IRequestHandler<ConfirmPhieuKpiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public ConfirmPhieuKpiCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(ConfirmPhieuKpiCommand request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuKpis
                .Include(p => p.NhanVien)
                .FirstOrDefaultAsync(p => p.IdPhieuKpi == request.IdPhieuKpi, cancellationToken);

            if (phieu == null)
                throw new ApiException($"Không tìm thấy phiếu KPI {request.IdPhieuKpi}");

            if (phieu.TrangThai != TrangThaiPhieuKpi.CHO_XAC_NHAN)
                throw new ApiException("Phiếu KPI không ở trang thái chờ xác nhận");

            if (phieu.NhanVien.IdTaiKhoan != request.TaiKhoanIdNhanVien)
                throw new ApiException("Bạn không có quyền xác nhân phiếu KPI này.");

            phieu.TrangThai = TrangThaiPhieuKpi.DANG_THUC_HIEN;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(phieu.IdPhieuKpi, "Xác nhân thành công. Đã chuyển sang trang thái Đang thực hiện");
        }
    }
}
