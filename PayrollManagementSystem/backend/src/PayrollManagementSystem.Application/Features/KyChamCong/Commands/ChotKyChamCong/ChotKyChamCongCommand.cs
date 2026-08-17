using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.KyChamCong.Commands.ChotKyChamCong
{
    public class ChotKyChamCongCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand, ITransactionalCommand
    {
        public int Thang { get; set; }
        public int Nam { get; set; }

        public string CacheKeyPrefix => $"{CacheKeyConstants.ChamCong},{CacheKeyConstants.KyChamCong}";
    }
}
