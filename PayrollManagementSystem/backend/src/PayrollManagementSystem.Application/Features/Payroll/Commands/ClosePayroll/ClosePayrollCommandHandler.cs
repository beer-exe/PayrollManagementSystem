using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ClosePayroll
{
    public class ClosePayrollCommandHandler : IRequestHandler<ClosePayrollCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ClosePayrollCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> Handle(ClosePayrollCommand request, CancellationToken cancellationToken)
        {
            var kyLuong = await _context.KyLuongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            if (kyLuong == null)
            {
                throw new ApiException($"Kỳ lương tháng {request.Thang}/{request.Nam} chưa được tạo!");
            }

            if (kyLuong.TrangThai != TrangThaiKyLuong.CHUA_CHOT)
            {
                throw new ApiException($"Kỳ lương tháng {request.Thang}/{request.Nam} đã được chốt hoặc thanh toán!");
            }

            // Kiểm tra xem đã có bảng lương nào chưa
            var hasBangLuong = await _context.BangLuongs.AnyAsync(x => x.IdKyLuong == kyLuong.IdKyLuong, cancellationToken);
            if (!hasBangLuong)
            {
                throw new ApiException($"Không thể chốt kỳ lương tháng {request.Thang}/{request.Nam} vì chưa có dữ liệu bảng lương (hãy ấn Tính lương trước).");
            }

            kyLuong.TrangThai = TrangThaiKyLuong.DA_CHOT;

            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, $"Đã chốt kỳ lương tháng {request.Thang}/{request.Nam} thành công.");
        }
    }
}
