using PayrollManagementSystem.Application.Features.Kpi.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.CreateKyKpi
{
    public class CreateKyKpiCommandHandler : IRequestHandler<CreateKyKpiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateKyKpiCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(CreateKyKpiCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.KyKpis.AnyAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);
            if (exists)
            {
                throw new ApiException($"Kỳ KPI tháng {request.Thang}/{request.Nam} đã tồn tại.");
            }

            var kyKpi = new KyKpi
            {
                IdKyKpi = Guid.NewGuid(),
                TenKyKpi = request.TenKyKpi,
                Thang = request.Thang,
                Nam = request.Nam,
                TrangThai = TrangThaiKyKpi.KHOI_TAO
            };

            _context.KyKpis.Add(kyKpi);

            var nhanViens = await _context.NhanViens
                .Where(nv => nv.TrangThai == TrangThaiNhanVien.DANG_LAM_VIEC)
                .ToListAsync(cancellationToken);

            foreach (var nv in nhanViens)
            {
                var phieu = new PhieuKpi
                {
                    IdPhieuKpi = Guid.NewGuid(),
                    IdKyKpi = kyKpi.IdKyKpi,
                    CccdNhanVien = nv.Cccd,
                    TongDiemKpi = 0,
                    HeSoP3 = 1.0m, // Mặc định là 1.0 (100%)
                    TrangThai = TrangThaiPhieuKpi.CHO_GIAO_MUC_TIEU
                };
                _context.PhieuKpis.Add(phieu);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(kyKpi.IdKyKpi, "Khởi tạo kỳ KPI thành công.");
        }
    }
}

