using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.CreateJobGrade
{
    public class CreateJobGradeCommand : IRequest<Response<string>>
    {
        public string TenNgachLuong { get; set; } = null!;
        public string? MoTa { get; set; }
    }
}
