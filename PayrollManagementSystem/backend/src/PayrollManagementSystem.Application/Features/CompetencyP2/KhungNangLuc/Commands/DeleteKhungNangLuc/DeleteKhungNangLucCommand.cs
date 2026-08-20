using MediatR;
using PayrollManagementSystem.Application.Common.Constants;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.DeleteKhungNangLuc
{
    public class DeleteKhungNangLucCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand
    {
        public Guid IdTieuChi { get; set; }

        public string CacheKeyPrefix => CacheKeyConstants.KhungNangLuc;
    }
}
