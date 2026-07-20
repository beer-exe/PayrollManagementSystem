using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.CalculateSoNgayNghi
{
    public class CalculateSoNgayNghiQueryHandler : IRequestHandler<CalculateSoNgayNghiQuery, Response<decimal>>
    {
        private readonly IApplicationDbContext _context;

        public CalculateSoNgayNghiQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<decimal>> Handle(CalculateSoNgayNghiQuery request, CancellationToken cancellationToken)
        {
            if (request.NgayKetThuc < request.NgayBatDau)
            {
                throw new ApiException("Ngày kết thúc không thể nhỏ hơn ngày bắt đầu.");
            }

            if (request.NgayBatDau.Year != request.NgayKetThuc.Year)
            {
                throw new ApiException("Ngày bắt đầu và ngày kết thúc phải cùng nằm trong một năm.");
            }

            var hasLich = await _context.LichLamViecs.AnyAsync(l => l.Nam == request.NgayBatDau.Year, cancellationToken);
            if (!hasLich)
            {
                throw new ApiException($"Chưa có lịch làm việc nào được tạo cho năm {request.NgayBatDau.Year}.");
            }

            if (!Enum.TryParse<LoaiNghi>(request.LoaiNghi, out var loaiNghi))
            {
                // Fallback to calendar days if LoaiNghi is invalid
                decimal diff = request.NgayKetThuc.DayNumber - request.NgayBatDau.DayNumber + 1;
                return new Response<decimal>(diff);
            }

            decimal maxSoNgay = 0;
            if (loaiNghi == LoaiNghi.NGHI_THAI_SAN)
            {
                maxSoNgay = request.NgayKetThuc.DayNumber - request.NgayBatDau.DayNumber + 1;
            }
            else
            {
                var chiTietLich = await _context.ChiTietLichLamViecs
                    .Where(c => c.Ngay >= request.NgayBatDau && c.Ngay <= request.NgayKetThuc)
                    .ToListAsync(cancellationToken);

                maxSoNgay = chiTietLich.Count(c => c.LoaiNgay == LoaiNgay.NGAY_LAM_VIEC);

                // Fallback in case HR hasn't created the schedule yet
                if (chiTietLich.Count == 0)
                {
                    maxSoNgay = request.NgayKetThuc.DayNumber - request.NgayBatDau.DayNumber + 1;
                }
            }

            return new Response<decimal>(maxSoNgay, "Tính toán thành công.");
        }
    }
}
