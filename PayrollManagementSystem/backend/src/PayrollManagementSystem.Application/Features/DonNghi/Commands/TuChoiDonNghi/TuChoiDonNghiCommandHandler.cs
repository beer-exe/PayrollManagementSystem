using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.TuChoiDonNghi
{
    public class TuChoiDonNghiCommandHandler : IRequestHandler<TuChoiDonNghiCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public TuChoiDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(TuChoiDonNghiCommand request, CancellationToken cancellationToken)
        {
            var donNghi = await _context.DonNghis
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (donNghi == null)
                throw new ApiException("Không tìm thấy đơn nghỉ.");

            if (donNghi.TrangThai != TrangThaiDonNghi.CHO_DUYET)
                throw new ApiException("Chỉ có thể từ chối đơn đang ở trạng thái 'Chờ duyệt'.");

            donNghi.TrangThai = TrangThaiDonNghi.TU_CHOI;
            donNghi.CccdNguoiDuyet = request.CccdNguoiDuyet;
            donNghi.LyDoTuChoi = request.LyDoTuChoi;
            donNghi.NgayDuyet = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Đã từ chối đơn nghỉ.");
        }
    }
}
