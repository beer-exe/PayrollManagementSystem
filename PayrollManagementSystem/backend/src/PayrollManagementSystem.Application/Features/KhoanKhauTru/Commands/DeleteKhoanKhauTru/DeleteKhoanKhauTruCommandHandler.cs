using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.DeleteKhoanKhauTru
{
    public class DeleteKhoanKhauTruCommandHandler : IRequestHandler<DeleteKhoanKhauTruCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteKhoanKhauTruCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(DeleteKhoanKhauTruCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.KhoanKhauTrus
                .FirstOrDefaultAsync(x => x.IdKhoanKhauTru == request.IdKhoanKhauTru, cancellationToken);

            if (entity == null)
            {
                throw new ApiException("Không tìm thấy khoản khấu trừ.");
            }

            _context.SoftRemove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Xóa khoản khấu trừ thành công.");
        }
    }
}
