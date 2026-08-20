using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Features.SeedData.Commands.SeedDemoData
{
    public class SeedDemoDataCommandHandler : IRequestHandler<SeedDemoDataCommand, Response<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public SeedDemoDataCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Response<string>> Handle(SeedDemoDataCommand request, CancellationToken cancellationToken)
        {
            if (await _context.NhanViens.AnyAsync(cancellationToken))
            {
                return new Response<string>("Database đã có dữ liệu, không thể seed.");
            }

            var vaiTroAdmin = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Admin", cancellationToken);

            if (vaiTroAdmin == null)
            {
                return new Response<string>("Cần có Role Admin trong database trước khi seed.");
            }

            var dummyPassword = "123abc@";

            var tk = new TaiKhoan
            {
                IdTaiKhoan = Guid.NewGuid(),
                TenTaiKhoan = "admin@company.com",
                MatKhauHash = _passwordHasher.HashPasswordEnhanced(dummyPassword),
                IdVaiTro = vaiTroAdmin.IdVaiTro,
                TrangThai = TrangThaiTaiKhoan.HOAT_DONG
            };
            _context.TaiKhoans.Add(tk);

            var nv = new NhanVien
            {
                Cccd = "001001001001",
                HoTen = "Nguyễn Văn Admin",
                Email = "admin@company.com",
                Sdt = "0900000000",
                GioiTinh = true,
                NgaySinh = new DateOnly(1990, 1, 1),
                DiaChi = "TP. Hồ Chí Minh",
                DanToc = "Kinh",
                ChuyenNganh = "Công nghệ thông tin",
                NgayVaoLam = new DateOnly(2023, 1, 1),
                TrangThai = TrangThaiNhanVien.DANG_LAM_VIEC,
                IdPb = null,
                IdTaiKhoan = tk.IdTaiKhoan
            };
            _context.NhanViens.Add(nv);

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<string>("Seed data thành công.", "Tạo dữ liệu thành công.");
        }
    }
}
