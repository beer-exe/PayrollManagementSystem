using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.DonNghi.Commands.DeleteMyDonNghi
{
    public class DeleteMyDonNghiCommand : IRequest<Response<bool>>, ITransactionalCommand, ICacheInvalidatorCommand
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }  // Set by controller from JWT for ownership check

        public string CacheKeyPrefix => "DonNghi";
    }
}
