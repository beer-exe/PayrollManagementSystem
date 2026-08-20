using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.CreateJobGrade
{
    public class CreateJobGradeCommand : IRequest<Response<string>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string TenNgachLuong { get; set; } = null!;
        public string? MoTa { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.JobGrades;
    }
}
