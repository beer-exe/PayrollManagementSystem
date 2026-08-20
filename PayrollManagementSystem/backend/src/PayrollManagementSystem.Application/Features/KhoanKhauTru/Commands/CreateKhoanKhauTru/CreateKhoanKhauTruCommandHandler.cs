using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.CreateKhoanKhauTru
{
    public class CreateKhoanKhauTruCommandHandler : IRequestHandler<CreateKhoanKhauTruCommand, Response<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateKhoanKhauTruCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Guid>> Handle(CreateKhoanKhauTruCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra trùng tên
            var existed = await _context.KhoanKhauTrus
                .AnyAsync(x => x.TenKhoanKhauTru == request.TenKhoanKhauTru, cancellationToken);

            if (existed)
            {
                throw new ApiException($"Khoản khấu trừ '{request.TenKhoanKhauTru}' đã tồn tại.");
            }

            var entity = new Domain.Models.KhoanKhauTru
            {
                TenKhoanKhauTru = request.TenKhoanKhauTru.Trim(),
                LoaiCongThuc = request.LoaiCongThuc,
                GiaTri = request.GiaTri,
                GhiChu = request.GhiChu?.Trim(),
                IsActive = request.IsActive,
            };

            _context.KhoanKhauTrus.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<Guid>(entity.IdKhoanKhauTru, "Thêm khoản khấu trừ thành công.");
        }
    }
}
