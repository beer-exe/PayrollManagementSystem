using MediatR;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SalarySteps.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Queries.GetActiveSalarySteps
{
    public class GetActiveSalaryStepsQueryHandler : IRequestHandler<GetActiveSalaryStepsQuery, Response<IEnumerable<SalaryStepDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetActiveSalaryStepsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Response<IEnumerable<SalaryStepDto>>> Handle(GetActiveSalaryStepsQuery request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var entities = await _context.BacLuongs
                .Where(x => x.IdNgachLuong == request.JobGradeId
                         && x.TrangThai == TrangThaiBacLuong.HIEU_LUC)
                .OrderBy(x => x.TenBacLuong)
                .ToListAsync(cancellationToken);

            var list = entities.Select(x => new SalaryStepDto
            {
                Id = x.IdBacLuong,
                StepName = x.TenBacLuong,
                P1Salary = x.LuongP1,
                EffectiveDate = x.NgayApDung.ToDateTime(TimeOnly.MinValue),
                EndDate = x.NgayKetThuc?.ToDateTime(TimeOnly.MinValue),
                Status = x.NgayApDung > today ? "CHUA_AP_DUNG" : x.TrangThai.ToString()
            }).ToList();

            return new Response<IEnumerable<SalaryStepDto>>(list);
        }
    }
}
