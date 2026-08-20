using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SalarySteps.Commands.UpdateSalaryStepVersion
{
    public class UpdateSalaryStepVersionCommand : IRequest<Response<string>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string JobGradeId { get; set; } = null!;
        public string StepName { get; set; } = null!;
        public decimal NewP1Salary { get; set; }
        public DateTime NewEffectiveDate { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.SalarySteps;
    }
}
