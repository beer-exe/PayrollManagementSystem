using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.SubmitPhieuKpi
{
    public class SubmitPhieuKpiCommandHandler : IRequestHandler<SubmitPhieuKpiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public SubmitPhieuKpiCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(SubmitPhieuKpiCommand request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuKpis
                .FirstOrDefaultAsync(p => p.IdPhieuKpi == request.IdPhieuKpi, cancellationToken);

            if (phieu == null)
                throw new ApiException($"Không tìm thấy phiếu KPI {request.IdPhieuKpi}");

            if (phieu.TrangThai != TrangThaiPhieuKpi.DANG_THUC_HIEN)
                throw new ApiException("Phiếu KPI phải ở trạng thái Đang thực hiện mới có thể nộp.");

            phieu.TrangThai = TrangThaiPhieuKpi.CHO_PHE_DUYET;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(phieu.IdPhieuKpi, "Nộp phiếu KPI thành công.");
        }
    }
}

