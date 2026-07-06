using MediatR;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.DeleteMucQuyDoi
{
    public class DeleteMucQuyDoiCommand : IRequest<Response<bool>>
    {
        public System.Guid IdQuyDoi { get; set; }

        public DeleteMucQuyDoiCommand(System.Guid idQuyDoi)
        {
            IdQuyDoi = idQuyDoi;
        }
    }
}
