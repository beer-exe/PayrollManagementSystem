using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.DuyetDonNghi
{
    public class DuyetDonNghiCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public string CccdNguoiDuyet { get; set; } = null!;
        public string CacheKeyPrefix => "DonNghi";
    }

    public class DuyetDonNghiCommandHandler : IRequestHandler<DuyetDonNghiCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public DuyetDonNghiCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(DuyetDonNghiCommand request, CancellationToken cancellationToken)
        {
            var donNghi = await _context.DonNghis
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (donNghi == null)
                throw new ApiException("Không tìm thấy đơn nghỉ.");

            if (donNghi.TrangThai != TrangThaiDonNghi.CHO_DUYET)
                throw new ApiException("Chỉ có thể duyệt đơn đang ở trạng thái 'Chờ duyệt'.");

            if (donNghi.LoaiNghi == LoaiNghi.NGHI_PHEP_NAM)
            {
                var nam = donNghi.NgayBatDau.Year;
                var ngayPhep = await _context.NgayPhepNhanViens
                    .FirstOrDefaultAsync(n => n.CccdNhanVien == donNghi.CccdNhanVien && n.Nam == nam, cancellationToken);

                if (ngayPhep == null)
                    throw new ApiException($"Nhân viên chưa được cấu hình quota phép năm {nam}. Vui lòng thiết lập trước khi duyệt.");

                if (ngayPhep.ConLai < donNghi.SoNgayNghi)
                    throw new ApiException($"Số ngày phép còn lại ({ngayPhep.ConLai}) không đủ cho đơn này ({donNghi.SoNgayNghi} ngày).");

                ngayPhep.DaSuDung += donNghi.SoNgayNghi;
            }

            donNghi.TrangThai = TrangThaiDonNghi.DA_DUYET;
            donNghi.CccdNguoiDuyet = request.CccdNguoiDuyet;
            donNghi.NgayDuyet = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, "Duyệt đơn nghỉ thành công.");
        }
    }
}
