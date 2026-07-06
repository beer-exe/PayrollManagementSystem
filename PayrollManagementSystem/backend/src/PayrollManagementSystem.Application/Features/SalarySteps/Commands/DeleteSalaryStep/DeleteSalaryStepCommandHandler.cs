using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.DeleteSalaryStep
{
    public class DeleteSalaryStepCommandHandler : IRequestHandler<DeleteSalaryStepCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteSalaryStepCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DeleteSalaryStepCommand request, CancellationToken cancellationToken)
        {
            var versions = await _context.BacLuongs
                .Where(x => x.IdChucVu == request.PositionId && x.TenBacLuong == request.StepName)
                .ToListAsync(cancellationToken);

            if (!versions.Any()) throw new Common.Exceptions.ApiException("LỖI: Không tìm thấy dữ liệu bậc lương.");

            var stepIds = versions.Select(v => v.IdBacLuong).ToList();

            bool isUsed = await _context.QuyetDinhNhanSus.AnyAsync(q => stepIds.Contains(q.IdBacLuongMoi), cancellationToken);
            if (isUsed)
                throw new Common.Exceptions.ApiException("LỖI: Bậc lương này hiện đang được sử dụng. Vui lòng tạo phiên bản mới.");

            _context.BacLuongs.RemoveRange(versions);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Đã xóa thành công.");
        }
    }
}
