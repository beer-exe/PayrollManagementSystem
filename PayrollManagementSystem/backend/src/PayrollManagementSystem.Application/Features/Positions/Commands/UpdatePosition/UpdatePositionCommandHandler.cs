using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Positions.Commands.UpdatePosition
{
    public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UpdatePositionCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
        {
            var chucVu = await _context.ChucVus.FindAsync(new object[] { request.IdChucVu }, cancellationToken);
            if (chucVu == null) throw new Common.Exceptions.ApiException("Chức vụ không tồn tại.");

            chucVu.TenChucVu = request.TenChucVu;
            chucVu.MoTaCongViec = request.MoTaCongViec;
            chucVu.IdNgachLuong = request.IdNgachLuong;
            chucVu.IdPhongBan = request.IdPhongBan;
            chucVu.IdChucVuQuanLy = request.IdChucVuQuanLy;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Cập nhật chức vụ thành công.");
        }
    }
}
