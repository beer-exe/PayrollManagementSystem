using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.DeleteMyDonNghi
{
    public class DeleteMyDonNghiCommandHandler : IRequestHandler<DeleteMyDonNghiCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMyDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DeleteMyDonNghiCommand request, CancellationToken cancellationToken)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == request.UserId, cancellationToken);

            if (taiKhoan?.NhanVien == null)
                throw new ApiException("Không tìm thấy thông tin nhân viên liên kết với tài khoản này.");

            var cccdHienTai = taiKhoan.NhanVien.Cccd;

            var donNghi = await _context.DonNghis
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (donNghi == null)
                throw new ApiException("Không tìm thấy đơn nghỉ.");

            if (donNghi.CccdNhanVien != cccdHienTai)
                throw new ApiException("Bạn không có quyền hủy đơn nghỉ này.");

            if (donNghi.TrangThai != TrangThaiDonNghi.CHO_DUYET)
                throw new ApiException("Chỉ có thể hủy đơn đang ở trạng thái 'Chờ duyệt'.");

            _context.SoftRemove(donNghi);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response<bool>(true, "Hủy đơn nghỉ thành công.");
        }
    }
}
