using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.DeleteSalaryStep
{
    public class DeleteSalaryStepCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string JobGradeId { get; set; } = null!;
        public string StepName { get; set; } = null!;

        public string CacheKeyPrefix => CacheKeyConstants.SalarySteps;
    }
}
