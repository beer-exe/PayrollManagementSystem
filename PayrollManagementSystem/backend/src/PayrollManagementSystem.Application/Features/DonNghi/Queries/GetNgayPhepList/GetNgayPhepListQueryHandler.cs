using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.DonNghi.Queries.GetNgayPhepList
{
    public class GetNgayPhepListQueryHandler : IRequestHandler<GetNgayPhepListQuery, Response<List<NgayPhepDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetNgayPhepListQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<List<NgayPhepDto>>> Handle(GetNgayPhepListQuery request, CancellationToken cancellationToken)
        {
            var query = _context.NgayPhepNhanViens
                .Include(n => n.NhanVien).ThenInclude(nv => nv.PhongBan)
                .Where(n => n.Nam == request.Nam);

            if (!string.IsNullOrWhiteSpace(request.IdPhongBan))
                query = query.Where(n => n.NhanVien.IdPb == request.IdPhongBan);

            var list = await query.ToListAsync(cancellationToken);

            var allCccd = list.Select(n => n.CccdNhanVien).Distinct().ToList();

            var now = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

            var quyetDinhs = await _context.QuyetDinhNhanSus
                .Include(qd => qd.ChucVuMoi)
                    .ThenInclude(cv => cv.PhongBan)
                .Where(qd => allCccd.Contains(qd.Cccd) 
                          && qd.TrangThai != TrangThaiQuyetDinh.HUY_BO
                          && qd.NgayHieuLuc <= now
                          && (qd.NgayHetHan == null || qd.NgayHetHan >= now))
                .ToListAsync(cancellationToken);
                
            var quyetDinhGroup = quyetDinhs
                .GroupBy(qd => qd.Cccd)
                .ToDictionary(
                    g => g.Key, 
                    g => g.OrderByDescending(qd => qd.NgayHieuLuc).FirstOrDefault()
                );

            var result = list.Select(n => 
            {
                quyetDinhGroup.TryGetValue(n.CccdNhanVien, out var qd);
                string? tenPhongBan = qd?.ChucVuMoi?.PhongBan?.TenPb ?? n.NhanVien.PhongBan?.TenPb;

                return new NgayPhepDto
                {
                    Id = n.Id,
                    CccdNhanVien = n.CccdNhanVien,
                    HoTenNhanVien = n.NhanVien.HoTen,
                    TenPhongBan = tenPhongBan,
                    Nam = n.Nam,
                    TongNgayPhep = n.TongNgayPhep,
                    DaSuDung = n.DaSuDung,
                    ConLai = n.ConLai,
                };
            })
            .OrderBy(n => n.TenPhongBan)
            .ThenBy(n => n.HoTenNhanVien)
            .ToList();

            return new Response<List<NgayPhepDto>>(result);
        }
    }
}
