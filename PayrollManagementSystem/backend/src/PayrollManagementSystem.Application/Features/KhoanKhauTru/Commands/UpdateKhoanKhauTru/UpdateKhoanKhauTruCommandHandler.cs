using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KhoanKhauTru.Commands.UpdateKhoanKhauTru
{
    public class UpdateKhoanKhauTruCommandHandler : IRequestHandler<UpdateKhoanKhauTruCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateKhoanKhauTruCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpdateKhoanKhauTruCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.KhoanKhauTrus
                .FirstOrDefaultAsync(x => x.IdKhoanKhauTru == request.IdKhoanKhauTru, cancellationToken);

            if (entity == null)
            {
                throw new ApiException("Không tìm thấy khoản khấu trừ.");
            }

            // Kiểm tra trùng tên với bản ghi khác
            var duplicateName = await _context.KhoanKhauTrus
                .AnyAsync(x => x.TenKhoanKhauTru == request.TenKhoanKhauTru
                               && x.IdKhoanKhauTru != request.IdKhoanKhauTru, cancellationToken);

            if (duplicateName)
            {
                throw new ApiException($"Khoản khấu trừ '{request.TenKhoanKhauTru}' đã tồn tại.");
            }

            entity.TenKhoanKhauTru = request.TenKhoanKhauTru.Trim();
            entity.LoaiCongThuc = request.LoaiCongThuc;
            entity.GiaTri = request.GiaTri;
            entity.GhiChu = request.GhiChu?.Trim();
            entity.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật khoản khấu trừ thành công.");
        }
    }
}
