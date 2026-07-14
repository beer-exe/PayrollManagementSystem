using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.Departments.Commands.ExpirePastDecisions
{
    public class ExpirePastDecisionsCommand : IRequest<bool>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public string CacheKeyPrefix => "Departments_";
    }
}
