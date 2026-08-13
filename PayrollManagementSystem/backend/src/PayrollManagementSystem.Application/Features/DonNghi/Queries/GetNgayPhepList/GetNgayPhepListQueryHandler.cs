using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.DonNghi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

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

            var list = await query
                .OrderBy(n => n.NhanVien.PhongBan!.TenPb)
                .ThenBy(n => n.NhanVien.HoTen)
                .ToListAsync(cancellationToken);

            var result = list.Select(n => new NgayPhepDto
            {
                Id = n.Id,
                CccdNhanVien = n.CccdNhanVien,
                HoTenNhanVien = n.NhanVien.HoTen,
                TenPhongBan = n.NhanVien.PhongBan?.TenPb,
                Nam = n.Nam,
                TongNgayPhep = n.TongNgayPhep,
                DaSuDung = n.DaSuDung,
                ConLai = n.ConLai,
            }).ToList();

            return new Response<List<NgayPhepDto>>(result);
        }
    }
}
