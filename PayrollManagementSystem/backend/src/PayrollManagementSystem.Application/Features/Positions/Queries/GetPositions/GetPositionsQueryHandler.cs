using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Positions.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Positions.Queries.GetPositions
{
    public class GetPositionsQueryHandler : IRequestHandler<GetPositionsQuery, Response<IEnumerable<PositionDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetPositionsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<PositionDto>>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ChucVus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = request.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.IdChucVu.ToLower().Contains(search) || x.TenChucVu.ToLower().Contains(search));
            }

            if (request.TrangThai.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.TrangThai.Value);
            }

            var positions = await query.OrderBy(x => x.IdChucVu).Select(cv => new PositionDto
            {
                IdChucVu = cv.IdChucVu,
                TenChucVu = cv.TenChucVu,
                MoTaCongViec = cv.MoTaCongViec,
                TrangThai = cv.TrangThai.ToString()
            }).ToListAsync(cancellationToken);

            return new Response<IEnumerable<PositionDto>>(positions, "Lấy danh sách chức vụ thành công.");
        }
    }
}
