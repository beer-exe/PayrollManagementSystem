using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.ChamCong.Queries.GetCaLamViecTrongNgay
{
    public class GetCaLamViecTrongNgayQueryHandler : IRequestHandler<GetCaLamViecTrongNgayQuery, Response<CaLamViecTrongNgayDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCaLamViecTrongNgayQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<CaLamViecTrongNgayDto>> Handle(GetCaLamViecTrongNgayQuery request, CancellationToken cancellationToken)
        {
            // First check explicitly assigned shift (PhanCongCa)
            var phanCong = await _context.PhanCongCas
                .Include(p => p.CaLamViec)
                .Where(p => p.CccdNhanVien == request.Cccd && p.NgayLamViec == request.Ngay)
                .FirstOrDefaultAsync(cancellationToken);

            if (phanCong != null && phanCong.IdCaLamViec != null && phanCong.CaLamViec != null)
            {
                return new Response<CaLamViecTrongNgayDto>(new CaLamViecTrongNgayDto
                {
                    GioVao = TimeOnly.FromTimeSpan(phanCong.CaLamViec.GioBatDau),
                    GioRa = TimeOnly.FromTimeSpan(phanCong.CaLamViec.GioKetThuc),
                    IsDayOff = false,
                    Source = "Phân công ca"
                });
            }

            // Fallback to default calendar
            var lich = await _context.ChiTietLichLamViecs
                .Include(l => l.CaLamViecMacDinh)
                .FirstOrDefaultAsync(l => l.Ngay == request.Ngay, cancellationToken);

            if (lich == null)
            {
                return new Response<CaLamViecTrongNgayDto>(new CaLamViecTrongNgayDto
                {
                    GioVao = new TimeOnly(8, 0),
                    GioRa = new TimeOnly(17, 0),
                    IsDayOff = false,
                    Source = "Mặc định (chưa cấu hình lịch)"
                });
            }

            if (lich.LoaiNgay == LoaiNgay.NGHI_LE || lich.LoaiNgay == LoaiNgay.NGHI_CUOI_TUAN)
            {
                return new Response<CaLamViecTrongNgayDto>(new CaLamViecTrongNgayDto
                {
                    GioVao = null,
                    GioRa = null,
                    IsDayOff = true,
                    Source = "Lịch công ty (Ngày nghỉ)"
                });
            }

            if (lich.CaLamViecMacDinh != null)
            {
                return new Response<CaLamViecTrongNgayDto>(new CaLamViecTrongNgayDto
                {
                    GioVao = TimeOnly.FromTimeSpan(lich.CaLamViecMacDinh.GioBatDau),
                    GioRa = TimeOnly.FromTimeSpan(lich.CaLamViecMacDinh.GioKetThuc),
                    IsDayOff = false,
                    Source = "Lịch công ty (Mặc định)"
                });
            }

            return new Response<CaLamViecTrongNgayDto>(new CaLamViecTrongNgayDto
            {
                GioVao = new TimeOnly(8, 0),
                GioRa = new TimeOnly(17, 0),
                IsDayOff = false,
                Source = "Mặc định hệ thống"
            });
        }
    }
}
