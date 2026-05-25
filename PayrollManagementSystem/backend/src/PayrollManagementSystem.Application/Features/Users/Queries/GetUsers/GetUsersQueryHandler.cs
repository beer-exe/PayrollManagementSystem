using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Users.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Response<IEnumerable<UserDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<IEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .Select(t => new UserDto
                {
                    IdTaiKhoan = t.IdTaiKhoan,
                    TenTaiKhoan = t.TenTaiKhoan,
                    Email = t.NhanVien != null ? t.NhanVien.Email : string.Empty,
                    HoTen = t.NhanVien != null ? t.NhanVien.HoTen : string.Empty,
                    TenVaiTro = t.VaiTro != null ? t.VaiTro.TenVaiTro : string.Empty,
                    IdVaiTro = t.IdVaiTro,
                    TrangThai = t.TrangThai.ToString()
                })
                .OrderByDescending(t => t.TenTaiKhoan)
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<UserDto>>(users, "Lấy danh sách tài khoản thành công.");
        }
    }
}