using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.DeleteLichLamViec
{
    public class DeleteLichLamViecCommandHandler : IRequestHandler<DeleteLichLamViecCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteLichLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(DeleteLichLamViecCommand request, CancellationToken cancellationToken)
        {
            var lich = await _context.LichLamViecs
                .Include(l => l.ChiTietLichLamViecs)
                .FirstOrDefaultAsync(l => l.IdLich == request.IdLich, cancellationToken);

            if (lich == null)
                throw new ApiException("Không tìm thấy lịch làm việc.");

            // Soft delete các chi tiết
            _context.SoftRemoveRange(lich.ChiTietLichLamViecs);

            // Soft delete lịch chính
            _context.SoftRemove(lich);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, $"Xóa lịch làm việc năm {lich.Nam} thành công.");
        }
    }
}
