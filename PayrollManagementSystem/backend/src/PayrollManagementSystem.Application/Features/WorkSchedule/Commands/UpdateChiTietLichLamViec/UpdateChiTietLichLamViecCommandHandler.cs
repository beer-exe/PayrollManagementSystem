using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Commands.UpdateChiTietLichLamViec
{
    public class UpdateChiTietLichLamViecCommandHandler : IRequestHandler<UpdateChiTietLichLamViecCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateChiTietLichLamViecCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(UpdateChiTietLichLamViecCommand request, CancellationToken cancellationToken)
        {
            var chiTiet = await _context.ChiTietLichLamViecs
                .FirstOrDefaultAsync(c => c.Id == request.IdChiTiet && !c.IsDeleted, cancellationToken);

            if (chiTiet == null)
            {
                throw new ApiException("Không tìm thấy chi tiết lịch làm việc.");
            }

            if (!PayrollManagementSystem.Domain.Extensions.EnumExtensions.TryGetValueFromDescription<LoaiNgay>(request.LoaiNgay, out var loaiNgay))
            {
                throw new ApiException("Loại ngày không hợp lệ.");
            }

            chiTiet.LoaiNgay = loaiNgay;
            
            if (loaiNgay == LoaiNgay.NGHI_CUOI_TUAN && string.IsNullOrWhiteSpace(request.TenNgayNghi))
            {
                chiTiet.TenNgayNghi = $"Nghỉ {chiTiet.Thu}";
            }
            else
            {
                chiTiet.TenNgayNghi = string.IsNullOrWhiteSpace(request.TenNgayNghi) ? null : request.TenNgayNghi.Trim();
            }
            
            // Adjust working hours based on day type
            if (loaiNgay == LoaiNgay.NGAY_LAM_VIEC)
            {
                chiTiet.SoGioLam = 8;
            }
            else
            {
                chiTiet.SoGioLam = 0;
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Cập nhật ngày thành công.");
        }
    }
}
