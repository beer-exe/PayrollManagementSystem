using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.DeleteJobGrade
{
    public class DeleteJobGradeCommandHandler : IRequestHandler<DeleteJobGradeCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DeleteJobGradeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DeleteJobGradeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NgachLuongs.FindAsync(new object[] { request.IdNgachLuong }, cancellationToken);
            if (entity == null) throw new Common.Exceptions.ApiException("Không tìm thấy ngạch lương.");

            var hasPositions = await _context.ChucVus.AnyAsync(x => x.IdNgachLuong == request.IdNgachLuong, cancellationToken);
            if (hasPositions) throw new Common.Exceptions.ApiException("Không thể xóa ngạch lương này vì đã được gán cho chức vụ.");

            _context.NgachLuongs.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Xóa ngạch lương thành công.");
        }
    }
}
