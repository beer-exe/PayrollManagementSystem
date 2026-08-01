using MediatR;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.DeleteBacThue
{
    public class DeleteBacThueCommandHandler : IRequestHandler<DeleteBacThueCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteBacThueCommandHandler(IApplicationDbContext context) { _context = context; }

        public async Task<Response<bool>> Handle(DeleteBacThueCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.BacThues.FindAsync(new object[] { request.IdBacThue }, cancellationToken)
                ?? throw new ApiException("Không tìm thấy bậc thuế.");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Xóa bậc thuế thành công.");
        }
    }
}
