using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Commands.DeleteMucQuyDoi
{
    public class DeleteMucQuyDoiCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public System.Guid IdQuyDoi { get; set; }

        public DeleteMucQuyDoiCommand(System.Guid idQuyDoi)
        {
            IdQuyDoi = idQuyDoi;
        }
        public string CacheKeyPrefix => CacheKeyConstants.MucQuyDoi;
    }
}
