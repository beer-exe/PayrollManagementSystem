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
            var nhanVienQuery = _context.NhanViens.Where(nv => !nv.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.CccdNhanVien))
            {
                nhanVienQuery = nhanVienQuery.Where(nv => nv.Cccd == request.CccdNhanVien);
            }

            var nhanViens = await nhanVienQuery.ToListAsync(cancellationToken);

            var cccds = nhanViens.Select(nv => nv.Cccd).ToList();

            var existingPhanCaList = await _context.NgayPhepNhanViens
                .Where(n => n.Nam == request.Nam && cccds.Contains(n.CccdNhanVien))
                .ToListAsync(cancellationToken);

            foreach (var nhanVien in nhanViens)
            {
                var existing = existingPhanCaList.FirstOrDefault(n => n.CccdNhanVien == nhanVien.Cccd);
                if (existing != null)
                {
                    existing.TongNgayPhep = request.TongNgayPhep;
                }
                else
                {
                    var ngayPhep = new Domain.Models.NgayPhepNhanVien
                    {
                        CccdNhanVien = nhanVien.Cccd,
                        Nam = request.Nam,
                        TongNgayPhep = request.TongNgayPhep,
                        DaSuDung = 0,
                    };
                    await _context.NgayPhepNhanViens.AddAsync(ngayPhep, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new Response<bool>(true, string.IsNullOrWhiteSpace(request.CccdNhanVien) 
                ? "Cấu hình phép cho toàn bộ nhân viên thành công." 
                : "Cập nhật ngày phép cho nhân viên thành công.");
        }
    }
}
