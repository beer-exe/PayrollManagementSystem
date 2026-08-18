using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.ConfirmBangLuong
{
    public class ConfirmBangLuongCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand, ITransactionalCommand
    {
        public Guid IdBangLuong { get; set; }

        public string CacheKeyPrefix => PayrollManagementSystem.Application.Common.Constants.CacheKeyConstants.Payroll;
    }
}
