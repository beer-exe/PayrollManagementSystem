using MediatR;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.DeleteMucQuyDoi
{
    public class DeleteMucQuyDoiCommandHandler : IRequestHandler<DeleteMucQuyDoiCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMucQuyDoiCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(DeleteMucQuyDoiCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.MucQuyDoiP2s.FindAsync(new object[] { request.IdQuyDoi }, cancellationToken);
            if (entity == null)
            {
                throw new ApiException($"Không tìm thấy cấu hình với Id {request.IdQuyDoi}");
            }

            _context.SoftRemove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Xóa thành công.");
        }
    }
}
