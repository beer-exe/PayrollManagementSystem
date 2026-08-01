using MediatR;
using PayrollManagementSystem.Application.Wrappers;

using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.JobGrades.Commands.CreateJobGrade
{
    public class CreateJobGradeCommand : IRequest<Response<string>>, ICacheInvalidatorCommand
    {
        public string TenNgachLuong { get; set; } = null!;
        public string? MoTa { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.JobGrades;
    }
}
