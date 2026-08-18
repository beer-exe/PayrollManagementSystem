using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.Payroll.Commands.RequestReviewBangLuong
{
    public class RequestReviewBangLuongCommand : IRequest<Response<bool>>, ICacheInvalidatorCommand, ITransactionalCommand
    {
        public Guid IdBangLuong { get; set; }
        public string LyDoKhieuNai { get; set; } = null!;

        public string CacheKeyPrefix => PayrollManagementSystem.Application.Common.Constants.CacheKeyConstants.Payroll;
    }
}
