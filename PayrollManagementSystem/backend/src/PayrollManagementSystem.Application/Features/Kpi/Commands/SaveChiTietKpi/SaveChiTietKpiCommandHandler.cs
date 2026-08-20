using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Kpi.Commands.SaveChiTietKpi
{
    public class SaveChiTietKpiCommandHandler : IRequestHandler<SaveChiTietKpiCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public SaveChiTietKpiCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(SaveChiTietKpiCommand request, CancellationToken cancellationToken)
        {
            var phieu = await _context.PhieuKpis
                .Include(p => p.ChiTietKpis)
                .FirstOrDefaultAsync(p => p.IdPhieuKpi == request.IdPhieuKpi, cancellationToken);

            if (phieu == null)
                throw new ApiException($"Không tìm thấy phiếu KPI {request.IdPhieuKpi}");

            if (phieu.TrangThai != Domain.Enums.TrangThaiPhieuKpi.DANG_THUC_HIEN)
                throw new ApiException("Chỉ có thể cập nhật tiến độ khi phiếu đang ở trạng thái đang thực hiện.");

            decimal tongDiem = 0;

            foreach (var item in request.ChiTietKpis)
            {
                if (item.IdChiTietKpi.HasValue)
                {
                    var existing = phieu.ChiTietKpis.FirstOrDefault(x => x.IdChiTietKpi == item.IdChiTietKpi.Value);
                    if (existing != null)
                    {
                        decimal tiLe = 0;
                        if (existing.ChiTieu > 0)
                        {
                            if (existing.LoaiTieuChi == Domain.Enums.LoaiTieuChiKpi.CANG_IT_CANG_TOT)
                            {
                                // Công thức: Tỷ lệ = (2 - (Thực tế / Chỉ tiêu)) * 100%
                                tiLe = (2m - (item.ThucTe / existing.ChiTieu)) * 100m;
                                // Giới hạn tỷ lệ nhỏ nhất là 0% để tránh điểm âm
                                if (tiLe < 0) tiLe = 0;
                            }
                            else
                            {
                                // Công thức: Tỷ lệ = (Thực tế / Chỉ tiêu) * 100%
                                tiLe = (item.ThucTe / existing.ChiTieu) * 100m;
                            }
                        }

                        var diemKpi = tiLe * existing.TrongSo / 100m;

                        tongDiem += diemKpi;

                        existing.ThucTe = item.ThucTe;
                        existing.TiLeHoanThanh = tiLe;
                        existing.DiemKpi = diemKpi;
                    }
                }
            }

            phieu.TongDiemKpi = tongDiem;
            // Tỷ lệ 1:1, ví dụ 105.5% = 1.055
            phieu.HeSoP3 = tongDiem / 100m;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(phieu.IdPhieuKpi, "Cập nhật tiến độ KPI thành công.");
        }
    }
}

