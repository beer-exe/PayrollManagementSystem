using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Positions.DTOs;
using PayrollManagementSystem.Domain.Enums;
using PayrollManagementSystem.Domain.Extensions;
using PayrollManagementSystem.Application.Wrappers;
namespace PayrollManagementSystem.Application.Features.Positions.Queries.GetPositions
{
    public class GetPositionsQueryHandler : IRequestHandler<GetPositionsQuery, Response<IEnumerable<PositionDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetPositionsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<PositionDto>>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ChucVus
                .AsNoTracking()
                .Include(x => x.NgachLuong)
                .Include(x => x.PhongBan)
                .Include(x => x.ChucVuQuanLy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = request.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.IdChucVu.ToLower().Contains(search) || x.TenChucVu.ToLower().Contains(search));
            }

            if (request.TrangThai.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.TrangThai.Value);
            }

            if (!string.IsNullOrEmpty(request.IdPhongBan))
            {
                query = query.Where(x => x.IdPhongBan == request.IdPhongBan);
            }

            var chucVus = await query.OrderBy(x => x.IdChucVu).ToListAsync(cancellationToken);
            
            var positions = chucVus.Select(cv => new PositionDto
            {
                IdChucVu = cv.IdChucVu,
                TenChucVu = cv.TenChucVu,
                MoTaCongViec = cv.MoTaCongViec,
                IdNgachLuong = cv.IdNgachLuong,
                TenNgachLuong = cv.NgachLuong != null ? cv.NgachLuong.TenNgachLuong : null,
                TrangThai = cv.TrangThai.GetDescription(),
                IdPhongBan = cv.IdPhongBan,
                TenPhongBan = cv.PhongBan != null ? cv.PhongBan.TenPb : null,
                IdChucVuQuanLy = cv.IdChucVuQuanLy,
                TenChucVuQuanLy = cv.ChucVuQuanLy != null ? cv.ChucVuQuanLy.TenChucVu : null
            });

            return new Response<IEnumerable<PositionDto>>(positions, "Lấy danh sách chức vụ thành công.");
        }
    }
}
