using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.UpdateNgayPhep
{
    public class UpdateNgayPhepCommandHandler : IRequestHandler<UpdateNgayPhepCommand, Response<bool>>
    {
        private readonly IApplicationDbContext _context;
        public UpdateNgayPhepCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<bool>> Handle(UpdateNgayPhepCommand request, CancellationToken cancellationToken)
        {
            var hasLich = await _context.LichLamViecs.AnyAsync(l => l.Nam == request.Nam, cancellationToken);
            if (!hasLich)
            {
                throw new ApiException($"Chưa có lịch làm việc nào được tạo cho năm {request.Nam}. Vui lòng tạo lịch làm việc trước khi cấu hình ngày phép.");
            }
            var existing = await _context.NgayPhepNhanViens
                .FirstOrDefaultAsync(n => n.CccdNhanVien == request.CccdNhanVien && n.Nam == request.Nam, cancellationToken);

            if (existing != null)
            {
                existing.TongNgayPhep = request.TongNgayPhep;
            }
            else
            {
                var nhanVien = await _context.NhanViens
                    .FirstOrDefaultAsync(nv => nv.Cccd == request.CccdNhanVien, cancellationToken);
                if (nhanVien == null)
                    throw new ApiException("Nhân viên không tồn tại.");

                var ngayPhep = new Domain.Models.NgayPhepNhanVien
                {
                    CccdNhanVien = request.CccdNhanVien,
                    Nam = request.Nam,
                    TongNgayPhep = request.TongNgayPhep,
                    DaSuDung = 0,
                };
                await _context.NgayPhepNhanViens.AddAsync(ngayPhep, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, existing != null
                ? "Cập nhật phép thành công."
                : "Tạo phép thành công.");
        }
    }
}
