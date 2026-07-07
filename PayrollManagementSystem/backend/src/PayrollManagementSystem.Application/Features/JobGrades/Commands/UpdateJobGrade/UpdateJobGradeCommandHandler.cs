using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.UpdateJobGrade
{
    public class UpdateJobGradeCommandHandler : IRequestHandler<UpdateJobGradeCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateJobGradeCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(UpdateJobGradeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NgachLuongs.FindAsync(new object[] { request.IdNgachLuong }, cancellationToken);
            if (entity == null) throw new Common.Exceptions.ApiException("Không tìm thấy ngạch lương.");

            entity.TenNgachLuong = request.TenNgachLuong;
            entity.MoTa = request.MoTa;
            entity.TrangThai = (TrangThaiNgachLuong)request.TrangThai;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Cập nhật thành công.");
        }
    }
}
