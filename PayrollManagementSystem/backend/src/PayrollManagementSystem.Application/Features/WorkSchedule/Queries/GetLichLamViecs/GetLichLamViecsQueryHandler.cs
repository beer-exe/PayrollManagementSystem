using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.WorkSchedule.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Extensions;

namespace PayrollManagementSystem.Application.Features.WorkSchedule.Queries.GetLichLamViecs
{
    public class GetLichLamViecsQueryHandler : IRequestHandler<GetLichLamViecsQuery, Response<List<LichLamViecDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetLichLamViecsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<List<LichLamViecDto>>> Handle(GetLichLamViecsQuery request, CancellationToken cancellationToken)
        {
            var lichList = await _context.LichLamViecs
                .OrderByDescending(l => l.Nam)
                .Select(l => new LichLamViecDto
                {
                    IdLich = l.IdLich,
                    Nam = l.Nam,
                    TrangThai = l.TrangThai.GetDescription(),
                    TongNgay = l.ChiTietLichLamViecs.Count(c => !c.IsDeleted),
                    TongNgayLam = l.ChiTietLichLamViecs.Count(c => !c.IsDeleted && c.LoaiNgay == Domain.Enums.LoaiNgay.NGAY_LAM_VIEC),
                    TongNgayNghiCuoiTuan = l.ChiTietLichLamViecs.Count(c => !c.IsDeleted && c.LoaiNgay == Domain.Enums.LoaiNgay.NGHI_CUOI_TUAN),
                    TongNgayLe = l.ChiTietLichLamViecs.Count(c => !c.IsDeleted && c.LoaiNgay == Domain.Enums.LoaiNgay.NGHI_LE),
                    GhiChu = l.GhiChu,
                    NguoiTao = l.CreatedBy.HasValue ? l.CreatedBy.Value.ToString() : null,
                    NgayTao = l.CreatedAt.DateTime
                })
                .ToListAsync(cancellationToken);

            return new Response<List<LichLamViecDto>>(lichList, "Lấy danh sách lịch làm việc thành công.");
        }
    }
}
