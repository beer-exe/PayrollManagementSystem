using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Users.Commands.ToggleUserStatus
{
    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public ToggleUserStatusCommandHandler(IApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Response<bool>> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.IdTaiKhoan, cancellationToken);

            if (taiKhoan == null) throw new ApiException("Tài khoản không tồn tại.");

            taiKhoan.TrangThai = taiKhoan.TrangThai == TrangThaiTaiKhoan.HOAT_DONG
                ? TrangThaiTaiKhoan.KHOA
                : TrangThaiTaiKhoan.HOAT_DONG;

            await _context.SaveChangesAsync(cancellationToken);

            if (taiKhoan.NhanVien != null && !string.IsNullOrEmpty(taiKhoan.NhanVien.Email))
            {
                string statusText = taiKhoan.TrangThai == TrangThaiTaiKhoan.KHOA ? "bị khóa" : "được mở khóa";
                string emailBody = $"Chào {taiKhoan.NhanVien.HoTen},<br/><br/>Tài khoản của bạn trên hệ thống HRMS vừa {statusText}. Vui lòng liên hệ Admin nếu có thắc mắc.";

                //_ = _emailService.SendAsync(taiKhoan.NhanVien.Email, "Thông báo thay đổi trạng thái tài khoản", emailBody);
            }

            return new Response<bool>(true, $"Đã chuyển trạng thái tài khoản thành {taiKhoan.TrangThai}.");
        }
    }
}
