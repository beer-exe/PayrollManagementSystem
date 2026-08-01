using MediatR;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.DeleteJobGrade
{
    public class DeleteJobGradeCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public string IdNgachLuong { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.JobGrades;
    }
}
