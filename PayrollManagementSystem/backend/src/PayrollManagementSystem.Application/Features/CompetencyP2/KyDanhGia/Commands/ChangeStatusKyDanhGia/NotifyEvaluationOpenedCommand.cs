using MediatR;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Commands.ChangeStatusKyDanhGia
{
    public class NotifyEvaluationOpenedCommand : IRequest<bool>
    {
        public Guid IdKyDanhGia { get; set; }

        public NotifyEvaluationOpenedCommand(Guid idKyDanhGia)
        {
            IdKyDanhGia = idKyDanhGia;
        }
    }
}
