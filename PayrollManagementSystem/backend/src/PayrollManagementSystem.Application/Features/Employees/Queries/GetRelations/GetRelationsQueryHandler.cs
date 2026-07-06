using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PayrollManagementSystem.Application.Features.Employees.Queries.GetRelations
{
    public class GetRelationsQueryHandler : IRequestHandler<GetRelationsQuery, Response<IEnumerable<RelationDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetRelationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<IEnumerable<RelationDto>>> Handle(GetRelationsQuery request, CancellationToken cancellationToken)
        {
            var relations = await _context.MoiQuanHes
                .Select(m => new RelationDto
                {
                    IdMqh = m.IdMqh,
                    TenQuanHe = m.TenQuanHe
                })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<RelationDto>>(relations, "Lấy danh sách mối quan hệ thành công.");
        }
    }
}
