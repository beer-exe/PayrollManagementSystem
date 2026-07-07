using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.DeleteKyDanhGia
{
    public class DeleteKyDanhGiaCommandHandler : IRequestHandler<DeleteKyDanhGiaCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteKyDanhGiaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DeleteKyDanhGiaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.KyDanhGias.FindAsync(new object[] { request.IdKyDanhGia }, cancellationToken);
            if (entity == null) return new Response<bool>("Không tìm thấy kỳ đánh giá.");

            bool hasTickets = await _context.PhieuDanhGiaNangLucs.AnyAsync(x => x.IdKyDanhGia == request.IdKyDanhGia, cancellationToken);
            if (hasTickets) 
            {
                return new Response<bool>("Không thể xóa kỳ đánh giá vì đã có phiếu đánh giá được tạo.");
            }

            _context.KyDanhGias.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Xóa kỳ đánh giá thành công.");
        }
    }
}
