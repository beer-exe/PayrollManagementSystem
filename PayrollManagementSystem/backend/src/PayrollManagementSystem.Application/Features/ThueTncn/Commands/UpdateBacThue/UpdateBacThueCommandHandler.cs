using MediatR;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Commands.UpdateBacThue
{
    public class UpdateBacThueCommandHandler : IRequestHandler<UpdateBacThueCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateBacThueCommandHandler(IApplicationDbContext context) { _context = context; }

        public async Task<Response<bool>> Handle(UpdateBacThueCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.BacThues.FindAsync(new object[] { request.IdBacThue }, cancellationToken)
                ?? throw new ApiException("Không tìm thấy bậc thuế.");

            entity.TuGia = request.TuGia;
            entity.DenGia = request.DenGia;
            entity.ThueSuat = request.ThueSuat;
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Cập nhật bậc thuế thành công.");
        }
    }
}
