using MediatR;
using PayrollManagementSystem.Application.Features.JobGrades.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Queries.GetJobGrades
{
    public class GetJobGradesQuery : IRequest<Response<IEnumerable<JobGradeDto>>>
    {
    }
}
