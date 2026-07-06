using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.Departments.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Response<IEnumerable<DepartmentDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDepartmentsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _context.PhongBans
                .Select(pb => new DepartmentDto
                {
                    IdPb = pb.IdPb,
                    TenPb = pb.TenPb
                })
                .ToListAsync(cancellationToken);

            return new Response<IEnumerable<DepartmentDto>>(departments, "Lấy danh sách phòng ban thành công.");
        }
    }
}
