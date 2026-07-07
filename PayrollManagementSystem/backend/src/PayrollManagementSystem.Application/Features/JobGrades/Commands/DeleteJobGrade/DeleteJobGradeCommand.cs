using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.DeleteJobGrade
{
    public class DeleteJobGradeCommand : IRequest<Response<bool>>
    {
        public string IdNgachLuong { get; set; } = null!;
    }
}
