using MediatR;

namespace PayrollManagementSystem.Application.Features.HrDecisions.Queries.GetNextDecisionCode
{
    public class GetNextDecisionCodeQuery : IRequest<string>
    {
        public string Type { get; set; } = null!;
    }
}
