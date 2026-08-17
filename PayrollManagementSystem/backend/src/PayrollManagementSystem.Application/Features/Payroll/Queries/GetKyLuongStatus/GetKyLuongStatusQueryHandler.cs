using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.Payroll.Queries.GetKyLuongStatus
{
    public class GetKyLuongStatusQueryHandler : IRequestHandler<GetKyLuongStatusQuery, Response<KyLuongStatusDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetKyLuongStatusQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<KyLuongStatusDto>> Handle(GetKyLuongStatusQuery request, CancellationToken cancellationToken)
        {
            var kyLuong = await _context.KyLuongs
                .FirstOrDefaultAsync(x => x.Thang == request.Thang && x.Nam == request.Nam, cancellationToken);

            var daysInMonth = DateTime.DaysInMonth(request.Nam, request.Thang);
            var defaultStartDate = new DateOnly(request.Nam, request.Thang, 1);
            var defaultEndDate = new DateOnly(request.Nam, request.Thang, daysInMonth);

            if (kyLuong == null)
            {
                return new Response<KyLuongStatusDto>(new KyLuongStatusDto
                {
                    Thang = request.Thang,
                    Nam = request.Nam,
                    TenKyLuong = $"Bảng lương tháng {request.Thang}/{request.Nam}",
                    NgayBatDau = defaultStartDate,
                    NgayKetThuc = defaultEndDate,
                    TrangThai = "CHUA_TAO",
                    TenTrangThai = "Chưa tạo",
                    IsLocked = false,
                    CoDuLieuBangLuong = false
                });
            }

            var hasBangLuong = await _context.BangLuongs
                .AnyAsync(x => x.IdKyLuong == kyLuong.IdKyLuong, cancellationToken);

            var isLocked = kyLuong.TrangThai == TrangThaiKyLuong.DA_CHOT || kyLuong.TrangThai == TrangThaiKyLuong.DA_THANH_TOAN;

            var dto = new KyLuongStatusDto
            {
                Thang = kyLuong.Thang,
                Nam = kyLuong.Nam,
                TenKyLuong = kyLuong.TenKyLuong,
                NgayBatDau = kyLuong.NgayBatDau,
                NgayKetThuc = kyLuong.NgayKetThuc,
                TrangThai = kyLuong.TrangThai.ToString(),
                TenTrangThai = kyLuong.TrangThai.GetDescription(),
                IsLocked = isLocked,
                CoDuLieuBangLuong = hasBangLuong
            };

            return new Response<KyLuongStatusDto>(dto);
        }
    }
}
