using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Profile.Commands.UpdateAvatar
{
    public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateAvatarCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Response<string>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new ApiException("Không tìm thấy thông tin phiên đăng nhập.");
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == userId, cancellationToken);

            if (taiKhoan == null)
            {
                throw new ApiException("Tài khoản không tồn tại.");
            }

            taiKhoan.UserAvatar = request.AvatarBase64;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>(taiKhoan.UserAvatar, "Cập nhật ảnh đại diện thành công.");
        }
    }
}
