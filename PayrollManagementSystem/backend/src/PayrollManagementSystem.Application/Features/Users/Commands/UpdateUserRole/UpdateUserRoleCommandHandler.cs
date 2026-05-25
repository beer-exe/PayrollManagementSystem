using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUserRoleCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(new object[] { request.IdTaiKhoan }, cancellationToken);
            if (taiKhoan == null) throw new ApiException("Tài khoản không tồn tại.");

            var vaiTroExists = await _context.VaiTros.AnyAsync(v => v.IdVaiTro == request.IdVaiTroMoi, cancellationToken);
            if (!vaiTroExists) throw new ApiException("Vai trò không hợp lệ.");

            taiKhoan.IdVaiTro = request.IdVaiTroMoi;
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật quyền hạn thành công.");
        }
    }
}
