using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.ThueTncn.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList
{
    public class GetBacThueListQueryHandler : IRequestHandler<GetBacThueListQuery, Response<List<BacThueDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetBacThueListQueryHandler(IApplicationDbContext context) { _context = context; }

        public async Task<Response<List<BacThueDto>>> Handle(GetBacThueListQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.BacThues
                .OrderBy(x => x.Bac)
                .Select(x => new BacThueDto
                {
                    IdBacThue = x.IdBacThue,
                    Bac = x.Bac,
                    TuGia = x.TuGia,
                    DenGia = x.DenGia,
                    ThueSuat = x.ThueSuat,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);
            return new Response<List<BacThueDto>>(list, "Lấy bảng thuế lũy tiến thành công.");
        }
    }
}
