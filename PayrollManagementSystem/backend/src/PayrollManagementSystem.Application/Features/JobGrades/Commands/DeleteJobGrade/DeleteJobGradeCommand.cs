using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.DeleteJobGrade
{
    public class DeleteJobGradeCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string IdNgachLuong { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.JobGrades;
    }
}
