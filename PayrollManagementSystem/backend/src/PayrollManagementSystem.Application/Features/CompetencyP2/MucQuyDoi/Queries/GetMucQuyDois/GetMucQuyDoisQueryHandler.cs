using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Queries.GetMucQuyDois
{
    public class GetMucQuyDoisQueryHandler : IRequestHandler<GetMucQuyDoisQuery, Response<IEnumerable<MucQuyDoiDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetMucQuyDoisQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<MucQuyDoiDto>>> Handle(GetMucQuyDoisQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.MucQuyDoiP2s
                .Select(x => new MucQuyDoiDto
                {
                    IdQuyDoi = x.IdQuyDoi,
                    XepLoai = x.XepLoai,
                    DiemToiThieu = x.DiemToiThieu,
                    DiemToiDa = x.DiemToiDa,
                    HeSoP2 = x.HeSoP2
                })
                .OrderByDescending(x => x.HeSoP2)
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<MucQuyDoiDto>>(data);
        }
    }
}
