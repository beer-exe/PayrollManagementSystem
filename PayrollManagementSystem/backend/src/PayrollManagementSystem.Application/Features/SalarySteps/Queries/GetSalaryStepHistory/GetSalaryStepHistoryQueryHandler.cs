using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetSalaryStepHistory
{
    public class GetSalaryStepHistoryQueryHandler : IRequestHandler<GetSalaryStepHistoryQuery, Response<IEnumerable<SalaryStepDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetSalaryStepHistoryQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<SalaryStepDto>>> Handle(GetSalaryStepHistoryQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.BacLuongs
                        .Where(x => x.IdChucVu == request.PositionId && x.TenBacLuong == request.StepName)
                        .OrderByDescending(x => x.NgayApDung)
                        .ToListAsync(cancellationToken);

            var history = entities.Select(x => new SalaryStepDto
            {
                Id = x.IdBacLuong,
                StepName = x.TenBacLuong,
                P1Salary = x.LuongP1,
                EffectiveDate = x.NgayApDung.ToDateTime(TimeOnly.MinValue),
                EndDate = x.NgayKetThuc?.ToDateTime(TimeOnly.MinValue),
                Status = x.TrangThai.ToString()
            }).ToList();

            return new Response<IEnumerable<SalaryStepDto>>(history);
        }
    }
}
