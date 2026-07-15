using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ChamCong.Commands.DeleteChamCong
{
    public class DeleteChamCongCommandHandler : IRequestHandler<DeleteChamCongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteChamCongCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(DeleteChamCongCommand request, CancellationToken cancellationToken)
        {
            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(cc => cc.Id == request.Id, cancellationToken);

            if (chamCong == null)
                throw new ApiException("Không tìm thấy bản ghi chấm công.");

            _context.SoftRemove(chamCong);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Xóa bản ghi chấm công thành công.");
        }
    }
}
