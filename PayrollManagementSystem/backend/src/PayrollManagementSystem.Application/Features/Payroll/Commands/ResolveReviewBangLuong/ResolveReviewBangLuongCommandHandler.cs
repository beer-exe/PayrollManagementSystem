using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Application.Features.Payroll.Commands.CalculatePayroll;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ResolveReviewBangLuong
{
    public class ResolveReviewBangLuongCommandHandler : IRequestHandler<ResolveReviewBangLuongCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISender _sender;

        public ResolveReviewBangLuongCommandHandler(IApplicationDbContext context, ISender sender)
        {
            _context = context;
            _sender = sender;
        }

        public async Task<Response<bool>> Handle(ResolveReviewBangLuongCommand request, CancellationToken cancellationToken)
        {
            var bangLuong = await _context.BangLuongs
                .Include(x => x.KyLuong)
                .FirstOrDefaultAsync(x => x.IdBangLuong == request.IdBangLuong, cancellationToken);

            if (bangLuong == null)
                throw new ApiException($"Không tìm thấy bảng lương có ID: {request.IdBangLuong}");

            if (bangLuong.KyLuong.TrangThai == TrangThaiKyLuong.DA_CHOT)
                throw new ApiException("Kỳ lương này đã bị khóa (Đã thanh toán), không thể xử lý khiếu nại.");

            if (bangLuong.TrangThai != TrangThaiBangLuong.YEU_CAU_XEM_XET)
                throw new ApiException("Bảng lương này không ở trạng thái yêu cầu xem xét.");

            if (request.Action == "REJECT")
            {
                // Ép trạng thái thành Đã xác nhận và lưu lý do từ chối
                bangLuong.TrangThai = TrangThaiBangLuong.DA_XAC_NHAN;
                bangLuong.PhanHoiKhieuNai = request.PhanHoiKhieuNai;
                await _context.SaveChangesAsync(cancellationToken);
                
                return new Response<bool>(true, "Đã từ chối khiếu nại và chuyển bảng lương sang trạng thái Đã xác nhận.");
            }
            else if (request.Action == "RECALCULATE")
            {
                // Đổi trạng thái bảng lương thành CHUA_XAC_NHAN và xóa phản hồi, xóa lý do khiếu nại
                // Sau đó gọi hàm tính lương của kỳ để tự động update số liệu mới cho bảng lương này
                bangLuong.TrangThai = TrangThaiBangLuong.CHUA_XAC_NHAN;
                bangLuong.LyDoKhieuNai = null;
                bangLuong.PhanHoiKhieuNai = null;
                await _context.SaveChangesAsync(cancellationToken);

                // Kích hoạt tính toán lại toàn bộ (chỉ những phiếu CHUA_XAC_NHAN mới bị ảnh hưởng)
                var calcCommand = new CalculatePayrollCommand
                {
                    Thang = bangLuong.Thang,
                    Nam = bangLuong.Nam
                };
                
                // Gửi command để chạy lại (Chú ý: vì CalculatePayrollCommand cũng là Transactional,
                // việc gọi sender.Send ở đây sẽ chạy trong một pipeline mới, nên ta lưu thay đổi trước).
                await _sender.Send(calcCommand, cancellationToken);

                return new Response<bool>(true, "Đã tính lại bảng lương dựa trên dữ liệu mới nhất. Trạng thái chuyển về Chưa xác nhận.");
            }
            else
            {
                throw new ApiException("Hành động không hợp lệ. Chỉ chấp nhận REJECT hoặc RECALCULATE.");
            }
        }
    }
}
