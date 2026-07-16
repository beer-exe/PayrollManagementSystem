using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.DeleteDonNghi
{
    public class DeleteDonNghiCommandHandler : IRequestHandler<DeleteDonNghiCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DeleteDonNghiCommand request, CancellationToken cancellationToken)
        {
            var donNghi = await _context.DonNghis
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (donNghi == null)
                throw new ApiException("Không tìm thấy đơn nghỉ.");

            if (donNghi.TrangThai != TrangThaiDonNghi.CHO_DUYET)
                throw new ApiException("Chỉ có thể xóa đơn đang ở trạng thái 'Chờ duyệt'.");

            _context.SoftRemove(donNghi);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Xóa đơn nghỉ thành công.");
        }
    }
}
