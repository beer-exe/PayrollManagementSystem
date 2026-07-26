using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkSchedule.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetChiTietLichLamViec
{
    public class GetChiTietLichLamViecQueryHandler : IRequestHandler<GetChiTietLichLamViecQuery, PagedResponse<List<ChiTietLichLamViecDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetChiTietLichLamViecQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<ChiTietLichLamViecDto>>> Handle(GetChiTietLichLamViecQuery request, CancellationToken cancellationToken)
        {
            var lichExists = await _context.LichLamViecs
                .AnyAsync(l => l.IdLich == request.IdLich, cancellationToken);

            if (!lichExists)
                throw new ApiException("Không tìm thấy lịch làm việc.");

            var query = _context.ChiTietLichLamViecs
                .Where(c => c.IdLich == request.IdLich && c.Ngay.Month == request.Thang)
                .OrderBy(c => c.Ngay);

            var totalRecords = await query.CountAsync(cancellationToken);

            var data = await query
                .Include(c => c.CaLamViecMacDinh)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ChiTietLichLamViecDto
                {
                    Id = c.Id,
                    Ngay = c.Ngay,
                    Thu = c.Thu,
                    LoaiNgay = c.LoaiNgay.GetDescription(),
                    TenNgayNghi = c.TenNgayNghi,
                    SoGioLam = c.SoGioLam,
                    IdCaLamViecMacDinh = c.IdCaLamViecMacDinh,
                    TenCaLamViecMacDinh = c.CaLamViecMacDinh != null ? c.CaLamViecMacDinh.TenCa : null
                })
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<ChiTietLichLamViecDto>>(
                data, request.PageNumber, request.PageSize, totalRecords,
                $"Lấy lịch làm việc tháng {request.Thang} thành công.");
        }
    }
}
