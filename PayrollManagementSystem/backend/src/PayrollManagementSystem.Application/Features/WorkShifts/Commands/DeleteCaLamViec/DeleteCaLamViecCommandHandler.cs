using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.WorkShifts.Commands.DeleteCaLamViec
{
    public class DeleteCaLamViecCommandHandler : IRequestHandler<DeleteCaLamViecCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCaLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(DeleteCaLamViecCommand request, CancellationToken cancellationToken)
        {
            var caLamViec = await _context.CaLamViecs
                .Include(c => c.KhungGioNghis)
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (caLamViec == null)
            {
                throw new ApiException("Không tìm thấy ca làm việc.");
            }

            bool isUsed = await _context.ChiTietLichLamViecs.AnyAsync(c => c.IdCaLamViecMacDinh == request.Id && !c.IsDeleted, cancellationToken)
                       || await _context.PhanCongCas.AnyAsync(p => p.IdCaLamViec == request.Id && !p.IsDeleted, cancellationToken);

            if (isUsed)
            {
                //throw new ApiException("Không thể xoá ca làm việc đã được gán vào lịch làm việc hoặc phân công ca. Vui lòng chọn 'Chỉnh sửa' và tắt Trạng thái (Vô hiệu hoá) thay vì xoá.");
                throw new ApiException("Không thể xoá ca làm việc đã được gán vào lịch làm việc hoặc phân công ca.");
            }

            // Using soft delete as per AGENTS.md rules
            _context.SoftRemove(caLamViec);

            if (caLamViec.KhungGioNghis.Any())
            {
                _context.SoftRemoveRange(caLamViec.KhungGioNghis);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Xóa ca làm việc thành công.");
        }
    }
}
