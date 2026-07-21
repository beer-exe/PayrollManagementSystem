using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PayrollManagementSystem.Application.Features.PhanCongCas.Commands.UpsertPhanCongCa
{
    public class UpsertPhanCongCaCommandHandler : IRequestHandler<UpsertPhanCongCaCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpsertPhanCongCaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpsertPhanCongCaCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.PhanCongCas
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.CccdNhanVien == request.CccdNhanVien 
                                       && p.NgayLamViec == request.NgayLamViec, cancellationToken);

            if (request.XoaPhanCong)
            {
                // Request to delete (Revert to default schedule)
                if (existing != null && !existing.IsDeleted)
                {
                    existing.IsDeleted = true;
                    await _context.SaveChangesAsync(cancellationToken);
                    return new Response<bool>(true, "Xoá phân công ca thành công.");
                }
                return new Response<bool>(true, "Không có phân công ca nào để xoá.");
            }

            // Verify CaLamViec exists if it's explicitly assigned a shift (not Nghỉ)
            if (request.IdCaLamViec != null)
            {
                var caLamViec = await _context.CaLamViecs.FirstOrDefaultAsync(c => c.Id == request.IdCaLamViec && !c.IsDeleted, cancellationToken);
                if (caLamViec == null)
                {
                    throw new ApiException("Ca làm việc không tồn tại hoặc đã bị xoá.");
                }
            }

            if (existing != null)
            {
                // Update or Reactivate
                existing.IdCaLamViec = request.IdCaLamViec;
                existing.IsDeleted = false; // Important: Reactivate if it was soft-deleted
                if (request.GhiChu != null)
                {
                    existing.GhiChu = request.GhiChu;
                }
            }
            else
            {
                // Create
                _context.PhanCongCas.Add(new PhanCongCa
                {
                    CccdNhanVien = request.CccdNhanVien,
                    NgayLamViec = request.NgayLamViec,
                    IdCaLamViec = request.IdCaLamViec,
                    GhiChu = request.GhiChu
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            string msg = request.IdCaLamViec == null ? "Đã gán ngày nghỉ ghi đè ca mặc định thành công." : "Cập nhật phân công ca thành công.";
            return new Response<bool>(true, msg);
        }
    }
}
