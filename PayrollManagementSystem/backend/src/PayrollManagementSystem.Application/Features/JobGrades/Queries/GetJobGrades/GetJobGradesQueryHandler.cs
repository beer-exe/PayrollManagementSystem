using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.JobGrades.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Queries.GetJobGrades
{
    public class GetJobGradesQueryHandler : IRequestHandler<GetJobGradesQuery, Response<IEnumerable<JobGradeDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetJobGradesQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<JobGradeDto>>> Handle(GetJobGradesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.NgachLuongs
                .OrderBy(x => x.TenNgachLuong)
                .ToListAsync(cancellationToken);

            var list = entities.Select(x => new JobGradeDto
            {
                IdNgachLuong = x.IdNgachLuong,
                TenNgachLuong = x.TenNgachLuong,
                MoTa = x.MoTa,
                TrangThai = (int)x.TrangThai
            }).ToList();

            return new Response<IEnumerable<JobGradeDto>>(list);
        }
    }
}
