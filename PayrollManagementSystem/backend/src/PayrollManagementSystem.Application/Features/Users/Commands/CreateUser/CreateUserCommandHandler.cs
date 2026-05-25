using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Response<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _context.TaiKhoans.AnyAsync(t => t.TenTaiKhoan == request.TenTaiKhoan, cancellationToken))
                throw new ApiException("Tên tài khoản đã tồn tại.");

            var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.Cccd == request.Cccd, cancellationToken);
            if (nhanVien == null) throw new ApiException("Không tìm thấy nhân viên với CCCD này.");
            if (nhanVien.IdTaiKhoan != null) throw new ApiException("Nhân viên này đã có tài khoản.");

            var taiKhoan = new TaiKhoan
            {
                TenTaiKhoan = request.TenTaiKhoan,
                MatKhauHash = _passwordHasher.HashPasswordEnhanced(request.MatKhau),
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG,
                DangNhapLanDau = true,
                IdVaiTro = request.IdVaiTro
            };

            _context.TaiKhoans.Add(taiKhoan);
            nhanVien.TaiKhoan = taiKhoan;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(taiKhoan.IdTaiKhoan, "Tạo tài khoản thành công.");
        }
    }
}
